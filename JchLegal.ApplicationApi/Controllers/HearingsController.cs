using JchLegal.ApplicationApi.Application.Command.Hearings;
using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.ApplicationApi.Application.Query.Hearings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JchLegal.ApplicationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HearingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HearingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HearingDto>>> GetHearings(
            [FromQuery] Guid? caseId,
            [FromQuery] string? status,
            [FromQuery] DateOnly? from,
            [FromQuery] DateOnly? to)
        {
            var result = await _mediator.Send(new GetHearingsRequest
            {
                CaseId = caseId,
                StatusCode = status,
                From = from,
                To = to
            });
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<HearingDto>> CreateHearing([FromBody] CreateHearingRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<HearingDto>> UpdateHearing(Guid id, [FromBody] UpdateHearingRequest request)
        {
            request.Id = id;
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> PatchHearingStatus(Guid id, [FromBody] PatchHearingStatusBody body)
        {
            await _mediator.Send(new PatchHearingStatusRequest { HearingId = id, StatusCode = body.Status });
            return NoContent();
        }
    }

    public class PatchHearingStatusBody
    {
        public string Status { get; set; } = string.Empty;
    }
}
