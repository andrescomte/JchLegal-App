using JchLegal.Domain.Models;

namespace JchLegal.Domain.Repository
{
    public interface IBitacoraRepository
    {
        Task<IEnumerable<BitacoraEntry>> GetByCaseAsync(Guid caseId, int tenantId, bool? visibleToClient);
        Task<BitacoraEntry?> GetByIdAsync(Guid id);
        Task<BitacoraEntry> CreateAsync(BitacoraEntry entry);
        Task<BitacoraEntry> UpdateAsync(BitacoraEntry entry);
        Task DeleteAsync(Guid id);
    }
}
