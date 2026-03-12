using JchLegal.Domain.Models;
using JchLegal.Domain.Repository;
using JchLegal.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.Infrastructure.Repository
{
    public class CasePartRepository : ICasePartRepository
    {
        private readonly JchLegalDbContext _db;

        public CasePartRepository(JchLegalDbContext db)
        {
            _db = db;
        }

        public async Task<CasePart?> GetByIdAsync(Guid partId, Guid caseId)
        {
            return await _db.CaseParts
                .Include(p => p.Role)
                .Include(p => p.ClientType)
                .FirstOrDefaultAsync(p => p.Id == partId && p.CaseId == caseId);
        }

        public async Task<CasePart> CreateAsync(CasePart part)
        {
            _db.CaseParts.Add(part);
            await _db.SaveChangesAsync();
            return part;
        }

        public async Task<CasePart> UpdateAsync(CasePart part)
        {
            await _db.SaveChangesAsync();
            return part;
        }

        public async Task DeleteAsync(CasePart part)
        {
            _db.CaseParts.Remove(part);
            await _db.SaveChangesAsync();
        }
    }
}
