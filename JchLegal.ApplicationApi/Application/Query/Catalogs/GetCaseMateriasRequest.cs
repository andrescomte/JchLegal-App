using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Infrastructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JchLegal.ApplicationApi.Application.Query.Catalogs
{
    public class GetCaseMateriasRequest : IRequest<List<CatalogItemDto>> { }

    public class GetCaseMateriasHandler : IRequestHandler<GetCaseMateriasRequest, List<CatalogItemDto>>
    {
        private readonly JchLegalDbContext _db;

        public GetCaseMateriasHandler(JchLegalDbContext db)
        {
            _db = db;
        }

        public async Task<List<CatalogItemDto>> Handle(GetCaseMateriasRequest request, CancellationToken cancellationToken)
        {
            return await _db.CaseMaterias
                .OrderBy(x => x.Id)
                .Select(x => new CatalogItemDto(x.Code, x.Name))
                .ToListAsync(cancellationToken);
        }
    }
}
