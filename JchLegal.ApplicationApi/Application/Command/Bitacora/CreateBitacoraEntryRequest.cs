using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Domain.Models;
using JchLegal.Domain.Repository;
using JchLegal.Domain.Services;
using JchLegal.Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.ApplicationApi.Application.Command.Bitacora
{
    public class CreateAttachmentRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public long? Size { get; set; }
        public string? MimeType { get; set; }
    }

    public class CreateBitacoraEntryRequest : IRequest<BitacoraEntryDto>
    {
        public Guid CaseId { get; set; }
        public DateOnly Date { get; set; }
        public string EventTypeCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool VisibleToClient { get; set; }
        public long CreatedBy { get; set; }
        public IEnumerable<CreateAttachmentRequest> Attachments { get; set; } = [];
    }

    public class CreateBitacoraEntryRequestHandler : IRequestHandler<CreateBitacoraEntryRequest, BitacoraEntryDto>
    {
        private readonly IBitacoraRepository _repo;
        private readonly ITenantContext _tenant;
        private readonly JchLegalDbContext _db;

        public CreateBitacoraEntryRequestHandler(IBitacoraRepository repo, ITenantContext tenant, JchLegalDbContext db)
        {
            _repo = repo;
            _tenant = tenant;
            _db = db;
        }

        public async Task<BitacoraEntryDto> Handle(CreateBitacoraEntryRequest request, CancellationToken cancellationToken)
        {
            var eventType = await _db.BitacoraEventTypes.FirstOrDefaultAsync(et => et.Code == request.EventTypeCode, cancellationToken)
                ?? throw new KeyNotFoundException($"EventType '{request.EventTypeCode}' not found.");

            var entry = new BitacoraEntry
            {
                CaseId = request.CaseId,
                Date = request.Date,
                EventTypeId = eventType.Id,
                Description = request.Description,
                VisibleToClient = request.VisibleToClient,
                CreatedBy = request.CreatedBy,
                Attachments = request.Attachments.Select(a => new Attachment
                {
                    Name = a.Name,
                    Url = a.Url,
                    Size = a.Size,
                    MimeType = a.MimeType
                }).ToList()
            };

            var created = await _repo.CreateAsync(entry);
            return MapToDto(created, eventType.Code);
        }

        internal static BitacoraEntryDto MapToDto(BitacoraEntry b, string? eventTypeCode = null) => new()
        {
            Id = b.Id,
            CaseId = b.CaseId,
            Date = b.Date,
            EventType = eventTypeCode ?? b.EventType?.Code ?? string.Empty,
            Description = b.Description,
            VisibleToClient = b.VisibleToClient,
            CreatedBy = b.CreatedBy,
            CreatedAt = b.CreatedAt,
            Attachments = b.Attachments.Select(a => new AttachmentDto
            {
                Id = a.Id,
                Name = a.Name,
                Url = a.Url,
                Size = a.Size,
                MimeType = a.MimeType,
                UploadedAt = a.UploadedAt
            })
        };
    }
}
