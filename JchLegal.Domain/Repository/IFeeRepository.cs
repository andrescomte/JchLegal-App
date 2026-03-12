using JchLegal.Domain.Models;

namespace JchLegal.Domain.Repository
{
    public interface IFeeRepository
    {
        Task<Fee?> GetByCaseAsync(Guid caseId, int tenantId);
        Task<IEnumerable<Fee>> GetAllAsync(int tenantId, string? statusFilter, Guid? clientId, long? lawyerId);
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<Fee?> GetFeeByIdAsync(Guid id);
        Task<Fee> CreateAsync(Fee fee);
    }
}
