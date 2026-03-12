using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.ApplicationApi.Application.Query.Catalogs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JchLegal.ApplicationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CatalogsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CatalogsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("case-statuses")]
        [ProducesResponseType(typeof(IEnumerable<CatalogItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCaseStatuses()
        {
            var result = await _mediator.Send(new GetCaseStatusesRequest());
            return Ok(result);
        }

        [HttpGet("case-materias")]
        [ProducesResponseType(typeof(IEnumerable<CatalogItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCaseMaterias()
        {
            var result = await _mediator.Send(new GetCaseMateriasRequest());
            return Ok(result);
        }
    }
}
