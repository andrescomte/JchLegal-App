using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Domain.Repository;
using JchLegal.Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.ApplicationApi.Application.Command.Bitacora
{
    public class UpdateBitacoraEntryRequest : IRequest<BitacoraEntryDto>
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public string EventTypeCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool VisibleToClient { get; set; }
    }

    public class UpdateBitacoraEntryRequestHandler : IRequestHandler<UpdateBitacoraEntryRequest, BitacoraEntryDto>
    {
        private readonly IBitacoraRepository _repo;
        private readonly JchLegalDbContext _db;

        public UpdateBitacoraEntryRequestHandler(IBitacoraRepository repo, JchLegalDbContext db)
        {
            _repo = repo;
            _db = db;
        }

        public async Task<BitacoraEntryDto> Handle(UpdateBitacoraEntryRequest request, CancellationToken cancellationToken)
        {
            var entry = await _repo.GetByIdAsync(request.Id)
                ?? throw new KeyNotFoundException($"BitacoraEntry {request.Id} not found.");

            var eventType = await _db.BitacoraEventTypes.FirstOrDefaultAsync(et => et.Code == request.EventTypeCode, cancellationToken)
                ?? throw new KeyNotFoundException($"EventType '{request.EventTypeCode}' not found.");

            entry.Date = request.Date;
            entry.EventTypeId = eventType.Id;
            entry.Description = request.Description;
            entry.VisibleToClient = request.VisibleToClient;

            var updated = await _repo.UpdateAsync(entry);
            return CreateBitacoraEntryRequestHandler.MapToDto(updated, eventType.Code);
        }
    }
}
