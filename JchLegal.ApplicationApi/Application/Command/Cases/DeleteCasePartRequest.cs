using JchLegal.Domain.Repository;
using MediatR;

namespace JchLegal.ApplicationApi.Application.Command.Cases
{
    public class DeleteCasePartCommand : IRequest
    {
        public Guid CaseId { get; set; }
        public Guid PartId { get; set; }
    }

    public class DeleteCasePartHandler : IRequestHandler<DeleteCasePartCommand>
    {
        private readonly ICasePartRepository _repo;

        public DeleteCasePartHandler(ICasePartRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(DeleteCasePartCommand request, CancellationToken cancellationToken)
        {
            var part = await _repo.GetByIdAsync(request.PartId, request.CaseId)
                ?? throw new KeyNotFoundException($"Part '{request.PartId}' not found in case '{request.CaseId}'.");

            await _repo.DeleteAsync(part);
        }
    }
}
