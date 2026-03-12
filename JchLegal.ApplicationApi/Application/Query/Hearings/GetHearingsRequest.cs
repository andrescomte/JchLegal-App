using JchLegal.ApplicationApi.Application.Command.Hearings;
using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Domain.Repository;
using JchLegal.Domain.Services;
using MediatR;

namespace JchLegal.ApplicationApi.Application.Query.Hearings
{
    public class GetHearingsRequest : IRequest<IEnumerable<HearingDto>>
    {
        public Guid? CaseId { get; set; }
        public string? StatusCode { get; set; }
        public DateOnly? From { get; set; }
        public DateOnly? To { get; set; }
    }

    public class GetHearingsRequestHandler : IRequestHandler<GetHearingsRequest, IEnumerable<HearingDto>>
    {
        private readonly IHearingRepository _repo;
        private readonly ITenantContext _tenant;

        public GetHearingsRequestHandler(IHearingRepository repo, ITenantContext tenant)
        {
            _repo = repo;
            _tenant = tenant;
        }

        public async Task<IEnumerable<HearingDto>> Handle(GetHearingsRequest request, CancellationToken cancellationToken)
        {
            var hearings = await _repo.GetAllAsync(_tenant.TenantId, request.CaseId, request.StatusCode, request.From, request.To);
            return hearings.Select(h => CreateHearingRequestHandler.MapToDto(h));
        }
    }
}
