using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Domain.Models;
using JchLegal.Domain.Repository;
using JchLegal.Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.ApplicationApi.Application.Command.Cases
{
    public class CreateCasePartCommand : IRequest<CasePartDto>
    {
        public Guid CaseId { get; set; }
        public string RoleCode { get; set; } = string.Empty;
        public string ClientTypeCode { get; set; } = string.Empty;
        public string? Nombre { get; set; }
        public string? Rut { get; set; }
        public string? RazonSocial { get; set; }
        public string? RepresentanteLegal { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
    }

    public class CreateCasePartHandler : IRequestHandler<CreateCasePartCommand, CasePartDto>
    {
        private readonly ICasePartRepository _repo;
        private readonly JchLegalDbContext _db;

        public CreateCasePartHandler(ICasePartRepository repo, JchLegalDbContext db)
        {
            _repo = repo;
            _db = db;
        }

        public async Task<CasePartDto> Handle(CreateCasePartCommand request, CancellationToken cancellationToken)
        {
            var caseExists = await _db.Cases.AnyAsync(c => c.Id == request.CaseId, cancellationToken);
            if (!caseExists)
                throw new KeyNotFoundException($"Case '{request.CaseId}' not found.");

            var role = await _db.CasePartRoles.FirstOrDefaultAsync(r => r.Code == request.RoleCode, cancellationToken)
                ?? throw new KeyNotFoundException($"Role '{request.RoleCode}' not found.");

            var clientType = await _db.ClientTypes.FirstOrDefaultAsync(t => t.Code == request.ClientTypeCode, cancellationToken)
                ?? throw new KeyNotFoundException($"ClientType '{request.ClientTypeCode}' not found.");

            var part = new CasePart
            {
                CaseId = request.CaseId,
                RoleId = role.Id,
                ClientTypeId = clientType.Id,
                Nombre = request.Nombre,
                Rut = request.Rut,
                RazonSocial = request.RazonSocial,
                RepresentanteLegal = request.RepresentanteLegal,
                Telefono = request.Telefono,
                Email = request.Email
            };

            var created = await _repo.CreateAsync(part);
            var full = await _repo.GetByIdAsync(created.Id, request.CaseId);
            return MapToDto(full!);
        }

        internal static CasePartDto MapToDto(CasePart p) => new()
        {
            Id = p.Id,
            Role = p.Role?.Code ?? string.Empty,
            Data = new CasePartDataDto
            {
                Type = p.ClientType?.Code,
                Nombre = p.Nombre,
                Rut = p.Rut,
                Telefono = p.Telefono,
                Email = p.Email,
                RazonSocial = p.RazonSocial,
                RepresentanteLegal = p.RepresentanteLegal
            }
        };
    }
}
