using JchLegal.Domain.Models;
using JchLegal.Domain.Repository;
using JchLegal.Domain.SeedWork;
using JchLegal.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.Infrastructure.Repository
{
    public class CaseRepository : ICaseRepository
    {
        private readonly JchLegalDbContext _db;

        public CaseRepository(JchLegalDbContext db)
        {
            _db = db;
        }

        public async Task<PagedResponse<Case>> GetAllAsync(int tenantId, string? statusCode, string? materiaCode, long? assignedLawyerId, Guid? clientId, string? search, int page, int pageSize)
        {
            var query = _db.Cases
                .Include(c => c.Materia)
                .Include(c => c.Status)
                .Include(c => c.Client).ThenInclude(cl => cl.ClientType)
                .Include(c => c.Parts).ThenInclude(p => p.Role)
                .Include(c => c.Parts).ThenInclude(p => p.ClientType)
                .Where(c => c.TenantId == tenantId);

            if (!string.IsNullOrWhiteSpace(statusCode))
                query = query.Where(c => c.Status.Code == statusCode);

            if (!string.IsNullOrWhiteSpace(materiaCode))
                query = query.Where(c => c.Materia.Code == materiaCode);

            if (assignedLawyerId.HasValue)
                query = query.Where(c => c.AssignedLawyerId == assignedLawyerId.Value);

            if (clientId.HasValue)
                query = query.Where(c => c.ClientId == clientId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c => (c.Expediente != null && c.Expediente.Contains(search)) || (c.Caratulado != null && c.Caratulado.Contains(search)));

            var total = await query.CountAsync();
            var data = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<Case>
            {
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };
        }

        public async Task<Case?> GetByIdAsync(Guid id, int tenantId)
        {
            return await _db.Cases
                .Include(c => c.Materia)
                .Include(c => c.Status)
                .Include(c => c.Client).ThenInclude(cl => cl.ClientType)
                .Include(c => c.Parts).ThenInclude(p => p.Role)
                .Include(c => c.Parts).ThenInclude(p => p.ClientType)
                .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);
        }

        public async Task<Case> CreateAsync(Case caseEntity)
        {
            _db.Cases.Add(caseEntity);
            await _db.SaveChangesAsync();
            return caseEntity;
        }

        public async Task<Case> UpdateAsync(Case caseEntity)
        {
            caseEntity.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return caseEntity;
        }

        public async Task PatchStatusAsync(Guid id, int tenantId, string statusCode)
        {
            var caseEntity = await _db.Cases.FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId)
                ?? throw new KeyNotFoundException($"Case {id} not found.");

            var status = await _db.CaseStatuses.FirstOrDefaultAsync(s => s.Code == statusCode)
                ?? throw new KeyNotFoundException($"Status '{statusCode}' not found.");

            caseEntity.StatusId = status.Id;
            caseEntity.UpdatedAt = DateTime.UtcNow;

            if (statusCode == "cerrado" && !caseEntity.ClosedDate.HasValue)
                caseEntity.ClosedDate = DateOnly.FromDateTime(DateTime.UtcNow);

            await _db.SaveChangesAsync();
        }
    }
}
