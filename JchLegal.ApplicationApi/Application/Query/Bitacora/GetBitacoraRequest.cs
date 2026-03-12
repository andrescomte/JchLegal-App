using JchLegal.ApplicationApi.Application.Command.Bitacora;
using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.Domain.Repository;
using JchLegal.Domain.Services;
using MediatR;

namespace JchLegal.ApplicationApi.Application.Query.Bitacora
{
    public class GetBitacoraRequest : IRequest<IEnumerable<BitacoraEntryDto>>
    {
        public Guid CaseId { get; set; }
        public bool? VisibleToClient { get; set; }
    }

    public class GetBitacoraRequestHandler : IRequestHandler<GetBitacoraRequest, IEnumerable<BitacoraEntryDto>>
    {
        private readonly IBitacoraRepository _repo;
        private readonly ITenantContext _tenant;

        public GetBitacoraRequestHandler(IBitacoraRepository repo, ITenantContext tenant)
        {
            _repo = repo;
            _tenant = tenant;
        }

        public async Task<IEnumerable<BitacoraEntryDto>> Handle(GetBitacoraRequest request, CancellationToken cancellationToken)
        {
            var entries = await _repo.GetByCaseAsync(request.CaseId, _tenant.TenantId, request.VisibleToClient);
            return entries.Select(b => CreateBitacoraEntryRequestHandler.MapToDto(b));
        }
    }
}
