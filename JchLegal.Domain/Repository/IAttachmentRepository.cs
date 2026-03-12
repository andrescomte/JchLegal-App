using JchLegal.Domain.Models;

namespace JchLegal.Domain.Repository
{
    public interface IAttachmentRepository
    {
        Task<Attachment?> GetByIdAsync(Guid id);
        Task<Attachment> CreateAsync(Attachment attachment);
        Task DeleteAsync(Attachment attachment);
    }
}
