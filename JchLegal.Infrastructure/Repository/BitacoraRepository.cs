using JchLegal.Domain.Models;
using JchLegal.Domain.Repository;
using JchLegal.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.Infrastructure.Repository
{
    public class BitacoraRepository : IBitacoraRepository
    {
        private readonly JchLegalDbContext _db;

        public BitacoraRepository(JchLegalDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<BitacoraEntry>> GetByCaseAsync(Guid caseId, int tenantId, bool? visibleToClient)
        {
            var query = _db.BitacoraEntries
                .Include(b => b.EventType)
                .Include(b => b.Attachments)
                .Where(b => b.CaseId == caseId && b.Case.TenantId == tenantId);

            if (visibleToClient.HasValue)
                query = query.Where(b => b.VisibleToClient == visibleToClient.Value);

            return await query.OrderByDescending(b => b.Date).ToListAsync();
        }

        public async Task<BitacoraEntry?> GetByIdAsync(Guid id)
        {
            return await _db.BitacoraEntries
                .Include(b => b.EventType)
                .Include(b => b.Attachments)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<BitacoraEntry> CreateAsync(BitacoraEntry entry)
        {
            _db.BitacoraEntries.Add(entry);
            await _db.SaveChangesAsync();
            return entry;
        }

        public async Task<BitacoraEntry> UpdateAsync(BitacoraEntry entry)
        {
            
            await _db.SaveChangesAsync();
            return entry;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entry = await _db.BitacoraEntries.FindAsync(id);
            if (entry != null)
            {
                _db.BitacoraEntries.Remove(entry);
                await _db.SaveChangesAsync();
            }
        }
    }
}
