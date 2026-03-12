using JchLegal.ApplicationApi.Application.Command.Hearings;
using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Domain.Repository;
using JchLegal.Domain.Services;
using MediatR;

namespace JchLegal.ApplicationApi.Application.Query.Hearings
{
    public class GetHearingsByCaseRequest : IRequest<IEnumerable<HearingDto>>
    {
        public Guid CaseId { get; set; }
    }

    public class GetHearingsByCaseRequestHandler : IRequestHandler<GetHearingsByCaseRequest, IEnumerable<HearingDto>>
    {
        private readonly IHearingRepository _repo;
        private readonly ITenantContext _tenant;

        public GetHearingsByCaseRequestHandler(IHearingRepository repo, ITenantContext tenant)
        {
            _repo = repo;
            _tenant = tenant;
        }

        public async Task<IEnumerable<HearingDto>> Handle(GetHearingsByCaseRequest request, CancellationToken cancellationToken)
        {
            var hearings = await _repo.GetByCaseAsync(request.CaseId, _tenant.TenantId);
            return hearings.Select(h => CreateHearingRequestHandler.MapToDto(h));
        }
    }
}
