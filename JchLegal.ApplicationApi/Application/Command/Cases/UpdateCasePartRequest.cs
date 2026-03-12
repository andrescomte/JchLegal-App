using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Domain.Repository;
using JchLegal.Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.ApplicationApi.Application.Command.Cases
{
    public class UpdateCasePartCommand : IRequest<CasePartDto>
    {
        public Guid CaseId { get; set; }
        public Guid PartId { get; set; }
        public string RoleCode { get; set; } = string.Empty;
        public string ClientTypeCode { get; set; } = string.Empty;
        public string? Nombre { get; set; }
        public string? Rut { get; set; }
        public string? RazonSocial { get; set; }
        public string? RepresentanteLegal { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
    }

    public class UpdateCasePartHandler : IRequestHandler<UpdateCasePartCommand, CasePartDto>
    {
        private readonly ICasePartRepository _repo;
        private readonly JchLegalDbContext _db;

        public UpdateCasePartHandler(ICasePartRepository repo, JchLegalDbContext db)
        {
            _repo = repo;
            _db = db;
        }

        public async Task<CasePartDto> Handle(UpdateCasePartCommand request, CancellationToken cancellationToken)
        {
            var part = await _repo.GetByIdAsync(request.PartId, request.CaseId)
                ?? throw new KeyNotFoundException($"Part '{request.PartId}' not found in case '{request.CaseId}'.");

            var role = await _db.CasePartRoles.FirstOrDefaultAsync(r => r.Code == request.RoleCode, cancellationToken)
                ?? throw new KeyNotFoundException($"Role '{request.RoleCode}' not found.");

            var clientType = await _db.ClientTypes.FirstOrDefaultAsync(t => t.Code == request.ClientTypeCode, cancellationToken)
                ?? throw new KeyNotFoundException($"ClientType '{request.ClientTypeCode}' not found.");

            part.RoleId = role.Id;
            part.ClientTypeId = clientType.Id;
            part.Nombre = request.Nombre;
            part.Rut = request.Rut;
            part.RazonSocial = request.RazonSocial;
            part.RepresentanteLegal = request.RepresentanteLegal;
            part.Telefono = request.Telefono;
            part.Email = request.Email;

            var updated = await _repo.UpdateAsync(part);
            return CreateCasePartHandler.MapToDto(updated);
        }
    }
}
