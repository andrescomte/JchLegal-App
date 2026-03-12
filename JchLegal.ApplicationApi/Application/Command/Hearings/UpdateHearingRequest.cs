using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Domain.Repository;
using JchLegal.Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.ApplicationApi.Application.Command.Hearings
{
    public class UpdateHearingRequest : IRequest<HearingDto>
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public string? Time { get; set; }
        public string? Juzgado { get; set; }
        public string StatusCode { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public short AlertDaysBefore { get; set; } = 3;
    }

    public class UpdateHearingRequestHandler : IRequestHandler<UpdateHearingRequest, HearingDto>
    {
        private readonly IHearingRepository _repo;
        private readonly JchLegalDbContext _db;

        public UpdateHearingRequestHandler(IHearingRepository repo, JchLegalDbContext db)
        {
            _repo = repo;
            _db = db;
        }

        public async Task<HearingDto> Handle(UpdateHearingRequest request, CancellationToken cancellationToken)
        {
            var hearing = await _repo.GetByIdAsync(request.Id)
                ?? throw new KeyNotFoundException($"Hearing {request.Id} not found.");

            var status = await _db.HearingStatuses.FirstOrDefaultAsync(s => s.Code == request.StatusCode, cancellationToken)
                ?? throw new KeyNotFoundException($"HearingStatus '{request.StatusCode}' not found.");

            TimeOnly? time = null;
            if (!string.IsNullOrWhiteSpace(request.Time) && TimeOnly.TryParse(request.Time, out var parsedTime))
                time = parsedTime;

            hearing.Title = request.Title;
            hearing.Date = request.Date;
            hearing.Time = time;
            hearing.Juzgado = request.Juzgado;
            hearing.StatusId = status.Id;
            hearing.Notes = request.Notes;
            hearing.AlertDaysBefore = request.AlertDaysBefore;

            var updated = await _repo.UpdateAsync(hearing);
            return CreateHearingRequestHandler.MapToDto(updated, status.Code);
        }
    }
}
