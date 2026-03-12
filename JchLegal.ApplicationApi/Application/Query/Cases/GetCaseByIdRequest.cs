using JchLegal.ApplicationApi.Application.Command.Cases;
using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Domain.Repository;
using JchLegal.Domain.Services;
using MediatR;

namespace JchLegal.ApplicationApi.Application.Query.Cases
{
    public class GetCaseByIdRequest : IRequest<CaseDto?>
    {
        public Guid Id { get; set; }
    }

    public class GetCaseByIdRequestHandler : IRequestHandler<GetCaseByIdRequest, CaseDto?>
    {
        private readonly ICaseRepository _repo;
        private readonly ITenantContext _tenant;

        public GetCaseByIdRequestHandler(ICaseRepository repo, ITenantContext tenant)
        {
            _repo = repo;
            _tenant = tenant;
        }

        public async Task<CaseDto?> Handle(GetCaseByIdRequest request, CancellationToken cancellationToken)
        {
            var c = await _repo.GetByIdAsync(request.Id, _tenant.TenantId);
            return c is null ? null : CreateCaseRequestHandler.MapToDto(c);
        }
    }
}
