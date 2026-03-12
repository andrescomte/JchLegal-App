using JchLegal.Domain.Models;
using JchLegal.Domain.SeedWork;

namespace JchLegal.Domain.Repository
{
    public interface ICaseRepository
    {
        Task<PagedResponse<Case>> GetAllAsync(int tenantId, string? statusCode, string? materiaCode, long? assignedLawyerId, Guid? clientId, string? search, int page, int pageSize);
        Task<Case?> GetByIdAsync(Guid id, int tenantId);
        Task<Case> CreateAsync(Case caseEntity);
        Task<Case> UpdateAsync(Case caseEntity);
        Task PatchStatusAsync(Guid id, int tenantId, string statusCode);
    }
}
