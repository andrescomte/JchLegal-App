using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.ApplicationApi.Application.Query.Fees;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JchLegal.ApplicationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FeesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FeesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FeeDto>>> GetFees(
            [FromQuery] string? status,
            [FromQuery] Guid? clientId,
            [FromQuery] long? assignedLawyerId)
        {
            var result = await _mediator.Send(new GetAllFeesRequest
            {
                StatusFilter = status,
                ClientId = clientId,
                AssignedLawyerId = assignedLawyerId
            });
            return Ok(result);
        }
    }
}
