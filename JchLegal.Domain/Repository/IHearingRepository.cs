using JchLegal.Domain.Models;

namespace JchLegal.Domain.Repository
{
    public interface IHearingRepository
    {
        Task<IEnumerable<Hearing>> GetAllAsync(int tenantId, Guid? caseId, string? statusCode, DateOnly? from, DateOnly? to);
        Task<IEnumerable<Hearing>> GetByCaseAsync(Guid caseId, int tenantId);
        Task<Hearing?> GetByIdAsync(Guid id);
        Task<Hearing> CreateAsync(Hearing hearing);
        Task<Hearing> UpdateAsync(Hearing hearing);
        Task PatchStatusAsync(Guid id, string statusCode);
    }
}
