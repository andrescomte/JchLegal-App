using JchLegal.Domain.Models;
using JchLegal.Domain.Repository;
using JchLegal.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.Infrastructure.Repository
{
    public class HearingRepository : IHearingRepository
    {
        private readonly JchLegalDbContext _db;

        public HearingRepository(JchLegalDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Hearing>> GetAllAsync(int tenantId, Guid? caseId, string? statusCode, DateOnly? from, DateOnly? to)
        {
            var query = _db.Hearings
                .Include(h => h.Status)
                .Include(h => h.Case)
                .Where(h => h.TenantId == tenantId);

            if (caseId.HasValue)
                query = query.Where(h => h.CaseId == caseId.Value);

            if (!string.IsNullOrWhiteSpace(statusCode))
                query = query.Where(h => h.Status.Code == statusCode);

            if (from.HasValue)
                query = query.Where(h => h.Date >= from.Value);

            if (to.HasValue)
                query = query.Where(h => h.Date <= to.Value);

            return await query.OrderBy(h => h.Date).ToListAsync();
        }

        public async Task<IEnumerable<Hearing>> GetByCaseAsync(Guid caseId, int tenantId)
        {
            return await _db.Hearings
                .Include(h => h.Status)
                .Include(h => h.Case)
                .Where(h => h.CaseId == caseId && h.TenantId == tenantId)
                .OrderBy(h => h.Date)
                .ToListAsync();
        }

        public async Task<Hearing?> GetByIdAsync(Guid id)
        {
            return await _db.Hearings
                .Include(h => h.Status)
                .Include(h => h.Case)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<Hearing> CreateAsync(Hearing hearing)
        {
            _db.Hearings.Add(hearing);
            await _db.SaveChangesAsync();
            return hearing;
        }

        public async Task<Hearing> UpdateAsync(Hearing hearing)
        {
            
            await _db.SaveChangesAsync();
            return hearing;
        }

        public async Task PatchStatusAsync(Guid id, string statusCode)
        {
            var hearing = await _db.Hearings.FindAsync(id)
                ?? throw new KeyNotFoundException($"Hearing {id} not found.");

            var status = await _db.HearingStatuses.FirstOrDefaultAsync(s => s.Code == statusCode)
                ?? throw new KeyNotFoundException($"HearingStatus '{statusCode}' not found.");

            hearing.StatusId = status.Id;
            await _db.SaveChangesAsync();
        }
    }
}
