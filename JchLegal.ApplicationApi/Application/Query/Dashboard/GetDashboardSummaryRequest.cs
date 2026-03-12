using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Domain.Services;
using JchLegal.Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.ApplicationApi.Application.Query.Dashboard
{
    public class GetDashboardSummaryRequest : IRequest<DashboardSummaryDto>
    {
    }

    public class GetDashboardSummaryRequestHandler : IRequestHandler<GetDashboardSummaryRequest, DashboardSummaryDto>
    {
        private readonly JchLegalDbContext _db;
        private readonly ITenantContext _tenant;

        public GetDashboardSummaryRequestHandler(JchLegalDbContext db, ITenantContext tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryRequest request, CancellationToken cancellationToken)
        {
            var tenantId = _tenant.TenantId;
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var activeCases = await _db.Cases
                .Include(c => c.Status)
                .CountAsync(c => c.TenantId == tenantId && c.Status.Code == "activo", cancellationToken);

            var totalClients = await _db.Clients
                .CountAsync(c => c.TenantId == tenantId, cancellationToken);

            var upcomingHearings = await _db.Hearings
                .Include(h => h.Status)
                .CountAsync(h => h.TenantId == tenantId && h.Date >= today && h.Status.Code == "programada", cancellationToken);

            var fees = await _db.Fees
                .Include(f => f.Payments)
                .Include(f => f.Case)
                .Where(f => f.Case.TenantId == tenantId)
                .ToListAsync(cancellationToken);

            var pendingFeesAmount = fees
                .Where(f =>
                {
                    var paid = f.Payments.Sum(p => p.Amount);
                    return paid < f.TotalAmount;
                })
                .Sum(f => f.TotalAmount - f.Payments.Sum(p => p.Amount));

            return new DashboardSummaryDto
            {
                ActiveCases = activeCases,
                TotalClients = totalClients,
                UpcomingHearings = upcomingHearings,
                PendingFeesAmount = pendingFeesAmount
            };
        }
    }
}
