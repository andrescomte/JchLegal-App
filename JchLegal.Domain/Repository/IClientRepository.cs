using JchLegal.Domain.Models;
using JchLegal.Domain.SeedWork;

namespace JchLegal.Domain.Repository
{
    public interface IClientRepository
    {
        Task<PagedResponse<Client>> GetAllAsync(int tenantId, string? search, string? typeCode, int page, int pageSize);
        Task<Client?> GetByIdAsync(Guid id, int tenantId);
        Task<Client?> GetByUserIdAsync(long userId, int tenantId);
        Task<Client> CreateAsync(Client client);
        Task<Client> UpdateAsync(Client client);
    }
}
