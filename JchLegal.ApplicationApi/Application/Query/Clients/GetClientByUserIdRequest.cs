using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Domain.Repository;
using JchLegal.Domain.Services;
using MediatR;

namespace JchLegal.ApplicationApi.Application.Query.Clients
{
    public class GetClientByUserIdRequest : IRequest<ClientDto?>
    {
        public long UserId { get; set; }
    }

    public class GetClientByUserIdRequestHandler : IRequestHandler<GetClientByUserIdRequest, ClientDto?>
    {
        private readonly IClientRepository _repo;
        private readonly ITenantContext _tenant;

        public GetClientByUserIdRequestHandler(IClientRepository repo, ITenantContext tenant)
        {
            _repo = repo;
            _tenant = tenant;
        }

        public async Task<ClientDto?> Handle(GetClientByUserIdRequest request, CancellationToken cancellationToken)
        {
            var c = await _repo.GetByUserIdAsync(request.UserId, _tenant.TenantId);
            if (c is null) return null;

            return new ClientDto
            {
                Id = c.Id,
                Type = c.ClientType?.Code ?? string.Empty,
                Nombre = c.Nombre,
                Rut = c.Rut,
                RazonSocial = c.RazonSocial,
                RutEmpresa = c.RutEmpresa,
                RepresentanteLegal = c.RepresentanteLegal,
                Contacto = new ClientContactDto
                {
                    Nombre = c.ContactoNombre,
                    Telefono = c.ContactoTelefono,
                    Email = c.ContactoEmail
                },
                UserId = c.UserId,
                AssignedCases = c.Cases.Select(cs => cs.Id),
                CreatedAt = c.CreatedAt
            };
        }
    }
}
