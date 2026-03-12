using JchLegal.ApplicationApi.Application.Command.Cases;
using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Domain.Repository;
using JchLegal.Domain.SeedWork;
using JchLegal.Domain.Services;
using MediatR;

namespace JchLegal.ApplicationApi.Application.Query.Cases
{
    public class GetCasesRequest : IRequest<PagedResponse<CaseDto>>
    {
        public string? StatusCode { get; set; }
        public string? MateriaCode { get; set; }
        public long? AssignedLawyerId { get; set; }
        public Guid? ClientId { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetCasesRequestHandler : IRequestHandler<GetCasesRequest, PagedResponse<CaseDto>>
    {
        private readonly ICaseRepository _repo;
        private readonly ITenantContext _tenant;

        public GetCasesRequestHandler(ICaseRepository repo, ITenantContext tenant)
        {
            _repo = repo;
            _tenant = tenant;
        }

        public async Task<PagedResponse<CaseDto>> Handle(GetCasesRequest request, CancellationToken cancellationToken)
        {
            var paged = await _repo.GetAllAsync(
                _tenant.TenantId,
                request.StatusCode,
                request.MateriaCode,
                request.AssignedLawyerId,
                request.ClientId,
                request.Search,
                request.Page,
                request.PageSize);

            return new PagedResponse<CaseDto>
            {
                Data = paged.Data.Select(CreateCaseRequestHandler.MapToDto),
                Page = paged.Page,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };
        }
    }
}
