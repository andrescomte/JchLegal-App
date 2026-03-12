using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Domain.Repository;
using JchLegal.Domain.Services;
using JchLegal.Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.ApplicationApi.Application.Command.Cases
{
    public class UpdateCaseRequest : IRequest<CaseDto>
    {
        public Guid Id { get; set; }
        public string? Expediente { get; set; }
        public string? Caratulado { get; set; }
        public string MateriaCode { get; set; } = string.Empty;
        public string? Juzgado { get; set; }
        public string StatusCode { get; set; } = string.Empty;
        public long? AssignedLawyerId { get; set; }
        public Guid ClientId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? ClosedDate { get; set; }
    }

    public class UpdateCaseRequestHandler : IRequestHandler<UpdateCaseRequest, CaseDto>
    {
        private readonly ICaseRepository _repo;
        private readonly ITenantContext _tenant;
        private readonly JchLegalDbContext _db;

        public UpdateCaseRequestHandler(ICaseRepository repo, ITenantContext tenant, JchLegalDbContext db)
        {
            _repo = repo;
            _tenant = tenant;
            _db = db;
        }

        public async Task<CaseDto> Handle(UpdateCaseRequest request, CancellationToken cancellationToken)
        {
            var caseEntity = await _repo.GetByIdAsync(request.Id, _tenant.TenantId)
                ?? throw new KeyNotFoundException($"Case {request.Id} not found.");

            //var clientExists = await _db.Clients.AnyAsync(c => c.Id == request.ClientId && c.TenantId == _tenant.TenantId, cancellationToken);
            //if (!clientExists)
            //    throw new KeyNotFoundException($"Client '{request.ClientId}' not found.");

            var materia = await _db.CaseMaterias.FirstOrDefaultAsync(m => m.Code == request.MateriaCode, cancellationToken)
                ?? throw new KeyNotFoundException($"Materia '{request.MateriaCode}' not found.");

            var status = await _db.CaseStatuses.FirstOrDefaultAsync(s => s.Code == request.StatusCode, cancellationToken)
                ?? throw new KeyNotFoundException($"Status '{request.StatusCode}' not found.");

            caseEntity.Expediente = request.Expediente;
            caseEntity.Caratulado = request.Caratulado;
            caseEntity.MateriaId = materia.Id;
            caseEntity.Juzgado = request.Juzgado;
            caseEntity.StatusId = status.Id;
            caseEntity.AssignedLawyerId = request.AssignedLawyerId;
            //caseEntity.ClientId = request.ClientId;
            caseEntity.StartDate = request.StartDate;
            caseEntity.ClosedDate = request.ClosedDate;

            var updated = await _repo.UpdateAsync(caseEntity);
            var full = await _repo.GetByIdAsync(updated.Id, _tenant.TenantId);
            return CreateCaseRequestHandler.MapToDto(full!);
        }
    }
}
