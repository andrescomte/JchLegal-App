using JchLegal.Domain.Models;
using JchLegal.Domain.Repository;
using JchLegal.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.Infrastructure.Repository
{
    public class FeeRepository : IFeeRepository
    {
        private readonly JchLegalDbContext _db;

        public FeeRepository(JchLegalDbContext db)
        {
            _db = db;
        }

        public async Task<Fee?> GetByCaseAsync(Guid caseId, int tenantId)
        {
            return await _db.Fees
                .Include(f => f.Concepto)
                .Include(f => f.Payments).ThenInclude(p => p.Method)
                .Include(f => f.Case)
                .FirstOrDefaultAsync(f => f.CaseId == caseId && f.Case.TenantId == tenantId);
        }

        public async Task<IEnumerable<Fee>> GetAllAsync(int tenantId, string? statusFilter, Guid? clientId, long? lawyerId)
        {
            var query = _db.Fees
                .Include(f => f.Concepto)
                .Include(f => f.Payments).ThenInclude(p => p.Method)
                .Include(f => f.Case).ThenInclude(c => c.Client)
                .Where(f => f.Case.TenantId == tenantId);

            if (clientId.HasValue)
                query = query.Where(f => f.Case.ClientId == clientId.Value);

            if (lawyerId.HasValue)
                query = query.Where(f => f.Case.AssignedLawyerId == lawyerId.Value);

            var fees = await query.ToListAsync();

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                fees = fees.Where(f =>
                {
                    var paid = f.Payments.Sum(p => p.Amount);
                    var feeStatus = paid >= f.TotalAmount ? "pagado" : paid > 0 ? "parcial" : "pendiente";
                    return feeStatus == statusFilter;
                }).ToList();
            }

            return fees;
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();
            return payment;
        }

        public async Task<Fee?> GetFeeByIdAsync(Guid id)
        {
            return await _db.Fees
                .Include(f => f.Concepto)
                .Include(f => f.Payments).ThenInclude(p => p.Method)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Fee> CreateAsync(Fee fee)
        {
            _db.Fees.Add(fee);
            await _db.SaveChangesAsync();
            return fee;
        }
    }
}
