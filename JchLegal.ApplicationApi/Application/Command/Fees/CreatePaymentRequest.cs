using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Domain.Models;
using JchLegal.Domain.Repository;
using JchLegal.Domain.Services;
using JchLegal.Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.ApplicationApi.Application.Command.Fees
{
    public class CreatePaymentRequest : IRequest<FeeDto>
    {
        public Guid FeeId { get; set; }
        public decimal Amount { get; set; }
        public DateOnly Date { get; set; }
        public string MethodCode { get; set; } = string.Empty;
        public string? Note { get; set; }
        public string? ReceiptUrl { get; set; }
    }

    public class CreatePaymentRequestHandler : IRequestHandler<CreatePaymentRequest, FeeDto>
    {
        private readonly IFeeRepository _repo;
        private readonly ITenantContext _tenant;
        private readonly JchLegalDbContext _db;

        public CreatePaymentRequestHandler(IFeeRepository repo, ITenantContext tenant, JchLegalDbContext db)
        {
            _repo = repo;
            _tenant = tenant;
            _db = db;
        }

        public async Task<FeeDto> Handle(CreatePaymentRequest request, CancellationToken cancellationToken)
        {
            var method = await _db.PaymentMethods.FirstOrDefaultAsync(m => m.Code == request.MethodCode, cancellationToken)
                ?? throw new KeyNotFoundException($"PaymentMethod '{request.MethodCode}' not found.");

            var payment = new Payment
            {
                FeeId = request.FeeId,
                Amount = request.Amount,
                Date = request.Date,
                MethodId = method.Id,
                Note = request.Note,
                ReceiptUrl = request.ReceiptUrl
            };

            await _repo.CreatePaymentAsync(payment);

            var fee = await _repo.GetFeeByIdAsync(request.FeeId)
                ?? throw new KeyNotFoundException($"Fee {request.FeeId} not found.");

            return MapFeeToDto(fee);
        }

        internal static FeeDto MapFeeToDto(Fee f)
        {
            var paid = f.Payments.Sum(p => p.Amount);
            var status = paid >= f.TotalAmount ? "pagado" : paid > 0 ? "parcial" : "pendiente";

            return new FeeDto
            {
                Id = f.Id,
                CaseId = f.CaseId,
                Concepto = f.Concepto?.Code ?? string.Empty,
                TotalAmount = f.TotalAmount,
                Currency = f.Currency,
                Status = status,
                Payments = f.Payments.Select(p => new PaymentDto
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    Date = p.Date,
                    Method = p.Method?.Code ?? string.Empty,
                    Note = p.Note,
                    ReceiptUrl = p.ReceiptUrl
                })
            };
        }
    }
}
