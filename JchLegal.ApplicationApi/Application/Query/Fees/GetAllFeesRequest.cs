using JchLegal.ApplicationApi.Application.Command.Fees;
using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Domain.Repository;
using JchLegal.Domain.Services;
using MediatR;

namespace JchLegal.ApplicationApi.Application.Query.Fees
{
    public class GetAllFeesRequest : IRequest<IEnumerable<FeeDto>>
    {
        public string? StatusFilter { get; set; }
        public Guid? ClientId { get; set; }
        public long? AssignedLawyerId { get; set; }
    }

    public class GetAllFeesRequestHandler : IRequestHandler<GetAllFeesRequest, IEnumerable<FeeDto>>
    {
        private readonly IFeeRepository _repo;
        private readonly ITenantContext _tenant;

        public GetAllFeesRequestHandler(IFeeRepository repo, ITenantContext tenant)
        {
            _repo = repo;
            _tenant = tenant;
        }

        public async Task<IEnumerable<FeeDto>> Handle(GetAllFeesRequest request, CancellationToken cancellationToken)
        {
            var fees = await _repo.GetAllAsync(_tenant.TenantId, request.StatusFilter, request.ClientId, request.AssignedLawyerId);
            return fees.Select(CreatePaymentRequestHandler.MapFeeToDto);
        }
    }
}
