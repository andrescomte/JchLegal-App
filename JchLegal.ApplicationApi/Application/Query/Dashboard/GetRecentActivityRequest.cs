using JchLegal.Domain.Services;
using JchLegal.Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.ApplicationApi.Application.Query.Dashboard
{
    public class RecentActivityItemDto
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public string? Expediente { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateOnly Date { get; set; }
    }

    public class GetRecentActivityRequest : IRequest<List<RecentActivityItemDto>>
    {
        public int Limit { get; set; } = 8;
    }

    public class GetRecentActivityHandler : IRequestHandler<GetRecentActivityRequest, List<RecentActivityItemDto>>
    {
        private readonly JchLegalDbContext _db;
        private readonly ITenantContext _tenant;

        public GetRecentActivityHandler(JchLegalDbContext db, ITenantContext tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        public async Task<List<RecentActivityItemDto>> Handle(GetRecentActivityRequest request, CancellationToken cancellationToken)
        {
            return await _db.BitacoraEntries
                .Include(b => b.Case)
                .Include(b => b.EventType)
                .Where(b => b.Case.TenantId == _tenant.TenantId)
                .OrderByDescending(b => b.CreatedAt)
                .Take(request.Limit)
                .Select(b => new RecentActivityItemDto
                {
                    Id = b.Id,
                    CaseId = b.CaseId,
                    Expediente = b.Case.Expediente,
                    EventType = b.EventType.Code,
                    Description = b.Description,
                    Date = b.Date
                })
                .ToListAsync(cancellationToken);
        }
    }
}
