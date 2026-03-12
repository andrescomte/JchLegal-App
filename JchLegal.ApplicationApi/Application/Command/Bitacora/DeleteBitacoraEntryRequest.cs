using JchLegal.Domain.Repository;
using MediatR;

namespace JchLegal.ApplicationApi.Application.Command.Bitacora
{
    public class DeleteBitacoraEntryRequest : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }

    public class DeleteBitacoraEntryRequestHandler : IRequestHandler<DeleteBitacoraEntryRequest, Unit>
    {
        private readonly IBitacoraRepository _repo;

        public DeleteBitacoraEntryRequestHandler(IBitacoraRepository repo)
        {
            _repo = repo;
        }

        public async Task<Unit> Handle(DeleteBitacoraEntryRequest request, CancellationToken cancellationToken)
        {
            await _repo.DeleteAsync(request.Id);
            return Unit.Value;
        }
    }
}
