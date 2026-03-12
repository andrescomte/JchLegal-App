using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Domain.Models;
using JchLegal.Domain.Repository;
using JchLegal.Domain.Services;
using JchLegal.Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.ApplicationApi.Application.Command.Cases
{
    public class CreateCasePartRequest
    {
        public string RoleCode { get; set; } = string.Empty;
        public string ClientTypeCode { get; set; } = string.Empty;
        public string? Nombre { get; set; }
        public string? Rut { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? RazonSocial { get; set; }
        public string? RepresentanteLegal { get; set; }
    }

    public class CreateCaseFeeRequest
    {
        public string ConceptoCode { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "CLP";
    }

    public class CreateCaseRequest : IRequest<CaseDto>
    {
        public string? Expediente { get; set; }
        public string? Caratulado { get; set; }
        public string MateriaCode { get; set; } = string.Empty;
        public string? Juzgado { get; set; }
        public string StatusCode { get; set; } = string.Empty;
        public long? AssignedLawyerId { get; set; }
        public Guid ClientId { get; set; }
        public DateOnly? StartDate { get; set; }
        public IEnumerable<CreateCasePartRequest> Parts { get; set; } = [];
        public CreateCaseFeeRequest? Fee { get; set; }
    }

    public class CreateCaseRequestHandler : IRequestHandler<CreateCaseRequest, CaseDto>
    {
        private readonly ICaseRepository _repo;
        private readonly IFeeRepository _feeRepo;
        private readonly ITenantContext _tenant;
        private readonly JchLegalDbContext _db;

        public CreateCaseRequestHandler(ICaseRepository repo, IFeeRepository feeRepo, ITenantContext tenant, JchLegalDbContext db)
        {
            _repo = repo;
            _feeRepo = feeRepo;
            _tenant = tenant;
            _db = db;
        }

        public async Task<CaseDto> Handle(CreateCaseRequest request, CancellationToken cancellationToken)
        {
            var clientExists = await _db.Clients.AnyAsync(c => c.Id == request.ClientId && c.TenantId == _tenant.TenantId, cancellationToken);
            if (!clientExists)
                throw new KeyNotFoundException($"Client '{request.ClientId}' not found.");

            var materia = await _db.CaseMaterias.FirstOrDefaultAsync(m => m.Code == request.MateriaCode, cancellationToken)
                ?? throw new KeyNotFoundException($"Materia '{request.MateriaCode}' not found.");

            var status = await _db.CaseStatuses.FirstOrDefaultAsync(s => s.Code == request.StatusCode, cancellationToken)
                ?? throw new KeyNotFoundException($"Status '{request.StatusCode}' not found.");

            var parts = new List<CasePart>();
            foreach (var partReq in request.Parts)
            {
                var role = await _db.CasePartRoles.FirstOrDefaultAsync(r => r.Code == partReq.RoleCode, cancellationToken)
                    ?? throw new KeyNotFoundException($"CasePartRole '{partReq.RoleCode}' not found.");
                var ct = await _db.ClientTypes.FirstOrDefaultAsync(t => t.Code == partReq.ClientTypeCode, cancellationToken)
                    ?? throw new KeyNotFoundException($"ClientType '{partReq.ClientTypeCode}' not found.");

                parts.Add(new CasePart
                {
                    RoleId = role.Id,
                    ClientTypeId = ct.Id,
                    Nombre = partReq.Nombre,
                    Rut = partReq.Rut,
                    Telefono = partReq.Telefono,
                    Email = partReq.Email,
                    RazonSocial = partReq.RazonSocial,
                    RepresentanteLegal = partReq.RepresentanteLegal
                });
            }

            var caseEntity = new Case
            {
                TenantId = _tenant.TenantId,
                Expediente = request.Expediente,
                Caratulado = request.Caratulado,
                MateriaId = materia.Id,
                Juzgado = request.Juzgado,
                StatusId = status.Id,
                AssignedLawyerId = request.AssignedLawyerId,
                ClientId = request.ClientId,
                StartDate = request.StartDate,
                Parts = parts,
                UpdatedAt = DateTime.UtcNow
            };

            var created = await _repo.CreateAsync(caseEntity);

            if (request.Fee != null)
            {
                var concepto = await _db.FeeConceptos.FirstOrDefaultAsync(fc => fc.Code == request.Fee.ConceptoCode, cancellationToken)
                    ?? throw new KeyNotFoundException($"FeeConcepto '{request.Fee.ConceptoCode}' not found.");

                var fee = new Fee
                {
                    CaseId = created.Id,
                    ConceptoId = concepto.Id,
                    TotalAmount = request.Fee.TotalAmount,
                    Currency = request.Fee.Currency
                };
                await _feeRepo.CreateAsync(fee);
            }

            var full = await _repo.GetByIdAsync(created.Id, _tenant.TenantId);
            return MapToDto(full!);
        }

        internal static CaseDto MapToDto(Case c) => new()
        {
            Id = c.Id,
            Expediente = c.Expediente,
            Caratulado = c.Caratulado,
            Materia = c.Materia?.Code ?? string.Empty,
            Juzgado = c.Juzgado,
            Status = c.Status?.Code ?? string.Empty,
            Parts = c.Parts.Select(p => new CasePartDto
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
            }),
            AssignedLawyerId = c.AssignedLawyerId,
            ClientId = c.ClientId,
            StartDate = c.StartDate,
            ClosedDate = c.ClosedDate,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        };
    }
}
