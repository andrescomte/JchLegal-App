using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Domain.Repository;
using JchLegal.Domain.SeedWork;
using JchLegal.Domain.Services;
using MediatR;

namespace JchLegal.ApplicationApi.Application.Query.Clients
{
    public class GetClientsRequest : IRequest<PagedResponse<ClientDto>>
    {
        public string? Search { get; set; }
        public string? TypeCode { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetClientsRequestHandler : IRequestHandler<GetClientsRequest, PagedResponse<ClientDto>>
    {
        private readonly IClientRepository _repo;
        private readonly ITenantContext _tenant;

        public GetClientsRequestHandler(IClientRepository repo, ITenantContext tenant)
        {
            _repo = repo;
            _tenant = tenant;
        }

        public async Task<PagedResponse<ClientDto>> Handle(GetClientsRequest request, CancellationToken cancellationToken)
        {
            var paged = await _repo.GetAllAsync(_tenant.TenantId, request.Search, request.TypeCode, request.Page, request.PageSize);

            return new PagedResponse<ClientDto>
            {
                Data = paged.Data.Select(c => new ClientDto
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
                }),
                Page = paged.Page,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };
        }
    }
}
