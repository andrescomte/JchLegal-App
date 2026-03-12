using JchLegal.ApplicationApi.Application.Command.Fees;
using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Domain.Repository;
using JchLegal.Domain.Services;
using MediatR;

namespace JchLegal.ApplicationApi.Application.Query.Fees
{
    public class GetCaseFeeRequest : IRequest<FeeDto?>
    {
        public Guid CaseId { get; set; }
    }

    public class GetCaseFeeRequestHandler : IRequestHandler<GetCaseFeeRequest, FeeDto?>
    {
        private readonly IFeeRepository _repo;
        private readonly ITenantContext _tenant;

        public GetCaseFeeRequestHandler(IFeeRepository repo, ITenantContext tenant)
        {
            _repo = repo;
            _tenant = tenant;
        }

        public async Task<FeeDto?> Handle(GetCaseFeeRequest request, CancellationToken cancellationToken)
        {
            var fee = await _repo.GetByCaseAsync(request.CaseId, _tenant.TenantId);
            return fee is null ? null : CreatePaymentRequestHandler.MapFeeToDto(fee);
        }
    }
}
