using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Domain.Models;
using JchLegal.Domain.Repository;
using JchLegal.Domain.Services;
using JchLegal.Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.ApplicationApi.Application.Command.Hearings
{
    public class CreateHearingRequest : IRequest<HearingDto>
    {
        public Guid CaseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public string? Time { get; set; }
        public string? Juzgado { get; set; }
        public string StatusCode { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public short AlertDaysBefore { get; set; } = 3;
    }

    public class CreateHearingRequestHandler : IRequestHandler<CreateHearingRequest, HearingDto>
    {
        private readonly IHearingRepository _repo;
        private readonly ITenantContext _tenant;
        private readonly JchLegalDbContext _db;

        public CreateHearingRequestHandler(IHearingRepository repo, ITenantContext tenant, JchLegalDbContext db)
        {
            _repo = repo;
            _tenant = tenant;
            _db = db;
        }

        public async Task<HearingDto> Handle(CreateHearingRequest request, CancellationToken cancellationToken)
        {
            var status = await _db.HearingStatuses.FirstOrDefaultAsync(s => s.Code == request.StatusCode, cancellationToken)
                ?? throw new KeyNotFoundException($"HearingStatus '{request.StatusCode}' not found.");

            TimeOnly? time = null;
            if (!string.IsNullOrWhiteSpace(request.Time) && TimeOnly.TryParse(request.Time, out var parsedTime))
                time = parsedTime;

            var hearing = new Hearing
            {
                CaseId = request.CaseId,
                TenantId = _tenant.TenantId,
                Title = request.Title,
                Date = request.Date,
                Time = time,
                Juzgado = request.Juzgado,
                StatusId = status.Id,
                Notes = request.Notes,
                AlertDaysBefore = request.AlertDaysBefore
            };

            var created = await _repo.CreateAsync(hearing);
            var full = await _repo.GetByIdAsync(created.Id);
            return MapToDto(full!, status.Code);
        }

        internal static HearingDto MapToDto(Hearing h, string? statusCode = null) => new()
        {
            Id = h.Id,
            CaseId = h.CaseId,
            CaseNumber = h.Case?.Expediente,
            Title = h.Title,
            Date = h.Date,
            Time = h.Time?.ToString("HH:mm"),
            Juzgado = h.Juzgado,
            Status = statusCode ?? h.Status?.Code ?? string.Empty,
            Notes = h.Notes,
            AlertDaysBefore = h.AlertDaysBefore
        };
    }
}
