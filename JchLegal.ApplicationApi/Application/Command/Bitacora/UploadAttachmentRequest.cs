using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Domain.Models;
using JchLegal.Domain.Repository;
using JchLegal.Domain.Services;
using JchLegal.Infrastructure.Context;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.ApplicationApi.Application.Command.Bitacora
{
    public class UploadAttachmentCommand : IRequest<AttachmentDto>
    {
        public Guid BitacoraEntryId { get; set; }
        public IFormFile File { get; set; } = null!;
    }

    public class UploadAttachmentHandler : IRequestHandler<UploadAttachmentCommand, AttachmentDto>
    {
        private readonly IAttachmentRepository _repo;
        private readonly IFileStorageService _storage;
        private readonly JchLegalDbContext _db;

        public UploadAttachmentHandler(IAttachmentRepository repo, IFileStorageService storage, JchLegalDbContext db)
        {
            _repo = repo;
            _storage = storage;
            _db = db;
        }

        public async Task<AttachmentDto> Handle(UploadAttachmentCommand request, CancellationToken cancellationToken)
        {
            var entryExists = await _db.BitacoraEntries.AnyAsync(b => b.Id == request.BitacoraEntryId, cancellationToken);
            if (!entryExists)
                throw new KeyNotFoundException($"BitacoraEntry '{request.BitacoraEntryId}' not found.");

            var url = await _storage.SaveAsync(
                request.File.OpenReadStream(),
                request.File.FileName,
                request.File.ContentType);

            var attachment = new Attachment
            {
                BitacoraEntryId = request.BitacoraEntryId,
                Name = request.File.FileName,
                Url = url,
                Size = request.File.Length,
                MimeType = request.File.ContentType
            };

            var created = await _repo.CreateAsync(attachment);

            return new AttachmentDto
            {
                Id = created.Id,
                Name = created.Name,
                Url = created.Url,
                Size = created.Size,
                MimeType = created.MimeType,
                UploadedAt = created.UploadedAt
            };
        }
    }
}
