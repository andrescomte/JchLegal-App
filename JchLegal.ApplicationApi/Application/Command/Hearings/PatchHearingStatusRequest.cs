using JchLegal.Domain.Repository;
using MediatR;

namespace JchLegal.ApplicationApi.Application.Command.Hearings
{
    public class PatchHearingStatusRequest : IRequest<Unit>
    {
        public Guid HearingId { get; set; }
        public string StatusCode { get; set; } = string.Empty;
    }

    public class PatchHearingStatusRequestHandler : IRequestHandler<PatchHearingStatusRequest, Unit>
    {
        private readonly IHearingRepository _repo;

        public PatchHearingStatusRequestHandler(IHearingRepository repo)
        {
            _repo = repo;
        }

        public async Task<Unit> Handle(PatchHearingStatusRequest request, CancellationToken cancellationToken)
        {
            await _repo.PatchStatusAsync(request.HearingId, request.StatusCode);
            return Unit.Value;
        }
    }
}
