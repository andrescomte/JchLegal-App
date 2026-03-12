using JchLegal.Domain.Models;
using JchLegal.Domain.Repository;
using JchLegal.Domain.SeedWork;
using JchLegal.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.Infrastructure.Repository
{
    public class ClientRepository : IClientRepository
    {
        private readonly JchLegalDbContext _db;

        public ClientRepository(JchLegalDbContext db)
        {
            _db = db;
        }

        public async Task<PagedResponse<Client>> GetAllAsync(int tenantId, string? search, string? typeCode, int page, int pageSize)
        {
            var query = _db.Clients
                .Include(c => c.ClientType)
                .Include(c => c.Cases)
                .Where(c => c.TenantId == tenantId);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c => c.Nombre.Contains(search) || (c.Rut != null && c.Rut.Contains(search)) || (c.RazonSocial != null && c.RazonSocial.Contains(search)));

            if (!string.IsNullOrWhiteSpace(typeCode))
                query = query.Where(c => c.ClientType.Code == typeCode);

            var total = await query.CountAsync();
            var data = await query
                .OrderBy(c => c.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<Client>
            {
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };
        }

        public async Task<Client?> GetByIdAsync(Guid id, int tenantId)
        {
            return await _db.Clients
                .Include(c => c.ClientType)
                .Include(c => c.Cases)
                .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);
        }

        public async Task<Client?> GetByUserIdAsync(long userId, int tenantId)
        {
            return await _db.Clients
                .Include(c => c.ClientType)
                .Include(c => c.Cases)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.TenantId == tenantId);
        }

        public async Task<Client> CreateAsync(Client client)
        {
            _db.Clients.Add(client);
            await _db.SaveChangesAsync();
            return client;
        }

        public async Task<Client> UpdateAsync(Client client)
        {
            
            await _db.SaveChangesAsync();
            return client;
        }
    }
}
