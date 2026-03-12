using JchLegal.ApplicationApi.Application.Command.Bitacora;
using JchLegal.ApplicationApi.Application.Command.Cases;
using JchLegal.ApplicationApi.Application.Command.Fees;
using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.ApplicationApi.Application.Query.Bitacora;
using JchLegal.ApplicationApi.Application.Query.Cases;
using JchLegal.ApplicationApi.Application.Query.Fees;
using JchLegal.ApplicationApi.Application.Query.Hearings;
using JchLegal.Domain.SeedWork;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JchLegal.ApplicationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CasesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CasesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ── Cases ──────────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<ActionResult<PagedResponse<CaseDto>>> GetCases(
            [FromQuery] string? status,
            [FromQuery] string? materia,
            [FromQuery] long? assignedLawyerId,
            [FromQuery] Guid? clientId,
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _mediator.Send(new GetCasesRequest
            {
                StatusCode = status,
                MateriaCode = materia,
                AssignedLawyerId = assignedLawyerId,
                ClientId = clientId,
                Search = search,
                Page = page,
                PageSize = pageSize
            });
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CaseDto>> GetCaseById(Guid id)
        {
            var result = await _mediator.Send(new GetCaseByIdRequest { Id = id });
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<CaseDto>> CreateCase([FromBody] CreateCaseRequest request)
        {
            var result = await _mediator.Send(request);
            return CreatedAtAction(nameof(GetCaseById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<CaseDto>> UpdateCase(Guid id, [FromBody] UpdateCaseRequest request)
        {
            request.Id = id;
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> PatchCaseStatus(Guid id, [FromBody] PatchCaseStatusBody body)
        {
            await _mediator.Send(new PatchCaseStatusRequest { CaseId = id, StatusCode = body.Status });
            return NoContent();
        }

        // ── Parts ──────────────────────────────────────────────────────────────

        [HttpPost("{caseId:guid}/parts")]
        [ProducesResponseType(typeof(CasePartDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<CasePartDto>> CreateCasePart(
            Guid caseId,
            [FromBody] CreateCasePartCommand request)
        {
            request.CaseId = caseId;
            var result = await _mediator.Send(request);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{caseId:guid}/parts/{partId:guid}")]
        [ProducesResponseType(typeof(CasePartDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CasePartDto>> UpdateCasePart(
            Guid caseId,
            Guid partId,
            [FromBody] UpdateCasePartCommand request)
        {
            request.CaseId = caseId;
            request.PartId = partId;
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpDelete("{caseId:guid}/parts/{partId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteCasePart(Guid caseId, Guid partId)
        {
            await _mediator.Send(new DeleteCasePartCommand { CaseId = caseId, PartId = partId });
            return NoContent();
        }

        // ── Bitacora ───────────────────────────────────────────────────────────

        [HttpGet("{caseId:guid}/bitacora")]
        public async Task<ActionResult<IEnumerable<BitacoraEntryDto>>> GetBitacora(
            Guid caseId,
            [FromQuery] bool? visibleToClient)
        {
            var result = await _mediator.Send(new GetBitacoraRequest
            {
                CaseId = caseId,
                VisibleToClient = visibleToClient
            });
            return Ok(result);
        }

        [HttpPost("{caseId:guid}/bitacora")]
        public async Task<ActionResult<BitacoraEntryDto>> CreateBitacoraEntry(
            Guid caseId,
            [FromBody] CreateBitacoraEntryRequest request)
        {
            request.CaseId = caseId;
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPut("{caseId:guid}/bitacora/{entryId:guid}")]
        public async Task<ActionResult<BitacoraEntryDto>> UpdateBitacoraEntry(
            Guid caseId,
            Guid entryId,
            [FromBody] UpdateBitacoraEntryRequest request)
        {
            request.Id = entryId;
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpDelete("{caseId:guid}/bitacora/{entryId:guid}")]
        public async Task<IActionResult> DeleteBitacoraEntry(Guid caseId, Guid entryId)
        {
            await _mediator.Send(new DeleteBitacoraEntryRequest { Id = entryId });
            return NoContent();
        }

        [HttpPost("{caseId:guid}/bitacora/{entryId:guid}/attachments")]
        [ProducesResponseType(typeof(AttachmentDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<AttachmentDto>> UploadAttachment(
            Guid caseId,
            Guid entryId,
            IFormFile file)
        {
            var result = await _mediator.Send(new UploadAttachmentCommand
            {
                BitacoraEntryId = entryId,
                File = file
            });
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpDelete("{caseId:guid}/bitacora/{entryId:guid}/attachments/{attachmentId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAttachment(Guid caseId, Guid entryId, Guid attachmentId)
        {
            await _mediator.Send(new DeleteAttachmentCommand { AttachmentId = attachmentId });
            return NoContent();
        }

        // ── Hearings ───────────────────────────────────────────────────────────

        [HttpGet("{caseId:guid}/hearings")]
        public async Task<ActionResult<IEnumerable<HearingDto>>> GetHearingsByCase(Guid caseId)
        {
            var result = await _mediator.Send(new GetHearingsByCaseRequest { CaseId = caseId });
            return Ok(result);
        }

        // ── Fees ───────────────────────────────────────────────────────────────

        [HttpGet("{caseId:guid}/fees")]
        public async Task<ActionResult<FeeDto>> GetCaseFee(Guid caseId)
        {
            var result = await _mediator.Send(new GetCaseFeeRequest { CaseId = caseId });
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpPost("{caseId:guid}/fees/payments")]
        public async Task<ActionResult<FeeDto>> CreatePayment(
            Guid caseId,
            [FromBody] CreatePaymentRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }

    public class PatchCaseStatusBody
    {
        public string Status { get; set; } = string.Empty;
    }
}
