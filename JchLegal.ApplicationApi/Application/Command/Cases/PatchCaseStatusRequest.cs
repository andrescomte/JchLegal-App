using JchLegal.Domain.Repository;
using JchLegal.Domain.Services;
using MediatR;

namespace JchLegal.ApplicationApi.Application.Command.Cases
{
    public class PatchCaseStatusRequest : IRequest<Unit>
    {
        public Guid CaseId { get; set; }
        public string StatusCode { get; set; } = string.Empty;
    }

    public class PatchCaseStatusRequestHandler : IRequestHandler<PatchCaseStatusRequest, Unit>
    {
        private readonly ICaseRepository _repo;
        private readonly ITenantContext _tenant;

        public PatchCaseStatusRequestHandler(ICaseRepository repo, ITenantContext tenant)
        {
            _repo = repo;
            _tenant = tenant;
        }

        public async Task<Unit> Handle(PatchCaseStatusRequest request, CancellationToken cancellationToken)
        {
            await _repo.PatchStatusAsync(request.CaseId, _tenant.TenantId, request.StatusCode);
            return Unit.Value;
        }
    }
}
