using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Domain.Repository;
using JchLegal.Domain.Services;
using JchLegal.Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.ApplicationApi.Application.Command.Clients
{
    public class UpdateClientRequest : IRequest<ClientDto>
    {
        public Guid Id { get; set; }
        public string TypeCode { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Rut { get; set; }
        public string? RazonSocial { get; set; }
        public string? RutEmpresa { get; set; }
        public string? RepresentanteLegal { get; set; }
        public string? ContactoNombre { get; set; }
        public string? ContactoTelefono { get; set; }
        public string? ContactoEmail { get; set; }
        public long? UserId { get; set; }
    }

    public class UpdateClientRequestHandler : IRequestHandler<UpdateClientRequest, ClientDto>
    {
        private readonly IClientRepository _repo;
        private readonly ITenantContext _tenant;
        private readonly JchLegalDbContext _db;

        public UpdateClientRequestHandler(IClientRepository repo, ITenantContext tenant, JchLegalDbContext db)
        {
            _repo = repo;
            _tenant = tenant;
            _db = db;
        }

        public async Task<ClientDto> Handle(UpdateClientRequest request, CancellationToken cancellationToken)
        {
            var client = await _repo.GetByIdAsync(request.Id, _tenant.TenantId)
                ?? throw new KeyNotFoundException($"Client {request.Id} not found.");

            var clientType = await _db.ClientTypes.FirstOrDefaultAsync(ct => ct.Code == request.TypeCode, cancellationToken)
                ?? throw new KeyNotFoundException($"ClientType '{request.TypeCode}' not found.");

            client.ClientTypeId = clientType.Id;
            client.Nombre = request.Nombre;
            client.Rut = request.Rut;
            client.RazonSocial = request.RazonSocial;
            client.RutEmpresa = request.RutEmpresa;
            client.RepresentanteLegal = request.RepresentanteLegal;
            client.ContactoNombre = request.ContactoNombre;
            client.ContactoTelefono = request.ContactoTelefono;
            client.ContactoEmail = request.ContactoEmail;
            client.UserId = request.UserId;

            var updated = await _repo.UpdateAsync(client);

            return new ClientDto
            {
                Id = updated.Id,
                Type = clientType.Code,
                Nombre = updated.Nombre,
                Rut = updated.Rut,
                RazonSocial = updated.RazonSocial,
                RutEmpresa = updated.RutEmpresa,
                RepresentanteLegal = updated.RepresentanteLegal,
                Contacto = new ClientContactDto
                {
                    Nombre = updated.ContactoNombre,
                    Telefono = updated.ContactoTelefono,
                    Email = updated.ContactoEmail
                },
                UserId = updated.UserId,
                AssignedCases = updated.Cases.Select(c => c.Id),
                CreatedAt = updated.CreatedAt
            };
        }
    }
}
