using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.ApplicationApi.Application.Query.Catalogs
{
    public class GetCaseStatusesRequest : IRequest<List<CatalogItemDto>> { }

    public class GetCaseStatusesHandler : IRequestHandler<GetCaseStatusesRequest, List<CatalogItemDto>>
    {
        private readonly JchLegalDbContext _db;

        public GetCaseStatusesHandler(JchLegalDbContext db)
        {
            _db = db;
        }

        public async Task<List<CatalogItemDto>> Handle(GetCaseStatusesRequest request, CancellationToken cancellationToken)
        {
            return await _db.CaseStatuses
                .OrderBy(x => x.Id)
                .Select(x => new CatalogItemDto(x.Code, x.Name))
                .ToListAsync(cancellationToken);
        }
    }
}
