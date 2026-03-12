using JchLegal.Domain.Repository;
using JchLegal.Domain.Services;
using MediatR;

namespace JchLegal.ApplicationApi.Application.Command.Bitacora
{
    public class DeleteAttachmentCommand : IRequest
    {
        public Guid AttachmentId { get; set; }
    }

    public class DeleteAttachmentHandler : IRequestHandler<DeleteAttachmentCommand>
    {
        private readonly IAttachmentRepository _repo;
        private readonly IFileStorageService _storage;

        public DeleteAttachmentHandler(IAttachmentRepository repo, IFileStorageService storage)
        {
            _repo = repo;
            _storage = storage;
        }

        public async Task Handle(DeleteAttachmentCommand request, CancellationToken cancellationToken)
        {
            var attachment = await _repo.GetByIdAsync(request.AttachmentId)
                ?? throw new KeyNotFoundException($"Attachment '{request.AttachmentId}' not found.");

            await _storage.DeleteAsync(attachment.Url);
            await _repo.DeleteAsync(attachment);
        }
    }
}
