using JchLegal.Domain.Models;
using JchLegal.Domain.Repository;
using JchLegal.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.Infrastructure.Repository
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly JchLegalDbContext _db;

        public AttachmentRepository(JchLegalDbContext db)
        {
            _db = db;
        }

        public async Task<Attachment?> GetByIdAsync(Guid id)
        {
            return await _db.Attachments.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Attachment> CreateAsync(Attachment attachment)
        {
            _db.Attachments.Add(attachment);
            await _db.SaveChangesAsync();
            return attachment;
        }

        public async Task DeleteAsync(Attachment attachment)
        {
            _db.Attachments.Remove(attachment);
            await _db.SaveChangesAsync();
        }
    }
}
