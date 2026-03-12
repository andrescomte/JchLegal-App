using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Domain.Models;
using JchLegal.Domain.Repository;
using JchLegal.Domain.Services;
using JchLegal.Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.ApplicationApi.Application.Command.Clients
{
    public class CreateClientRequest : IRequest<ClientDto>
    {
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

    public class CreateClientRequestHandler : IRequestHandler<CreateClientRequest, ClientDto>
    {
        private readonly IClientRepository _repo;
        private readonly ITenantContext _tenant;
        private readonly JchLegalDbContext _db;

        public CreateClientRequestHandler(IClientRepository repo, ITenantContext tenant, JchLegalDbContext db)
        {
            _repo = repo;
            _tenant = tenant;
            _db = db;
        }

        public async Task<ClientDto> Handle(CreateClientRequest request, CancellationToken cancellationToken)
        {
            var clientType = await _db.ClientTypes.FirstOrDefaultAsync(ct => ct.Code == request.TypeCode, cancellationToken)
                ?? throw new KeyNotFoundException($"ClientType '{request.TypeCode}' not found.");

            var client = new Client
            {
                TenantId = _tenant.TenantId,
                ClientTypeId = clientType.Id,
                Nombre = request.Nombre,
                Rut = request.Rut,
                RazonSocial = request.RazonSocial,
                RutEmpresa = request.RutEmpresa,
                RepresentanteLegal = request.RepresentanteLegal,
                ContactoNombre = request.ContactoNombre,
                ContactoTelefono = request.ContactoTelefono,
                ContactoEmail = request.ContactoEmail,
                UserId = request.UserId
            };

            var created = await _repo.CreateAsync(client);
            return MapToDto(created, clientType.Code);
        }

        private static ClientDto MapToDto(Client c, string typeCode) => new()
        {
            Id = c.Id,
            Type = typeCode,
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
