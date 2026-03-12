using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.ApplicationApi.Application.Query.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JchLegal.ApplicationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
        {
            var result = await _mediator.Send(new GetDashboardSummaryRequest());
            return Ok(result);
        }

        [HttpGet("recent-activity")]
        [ProducesResponseType(typeof(IEnumerable<RecentActivityItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRecentActivity([FromQuery] int limit = 8)
        {
            var result = await _mediator.Send(new GetRecentActivityRequest { Limit = limit });
            return Ok(result);
        }
    }
}
