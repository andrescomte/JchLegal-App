using JchLegal.Domain.Models;

namespace JchLegal.Domain.Repository
{
    public interface ICasePartRepository
    {
        Task<CasePart?> GetByIdAsync(Guid partId, Guid caseId);
        Task<CasePart> CreateAsync(CasePart part);
        Task<CasePart> UpdateAsync(CasePart part);
        Task DeleteAsync(CasePart part);
    }
}
