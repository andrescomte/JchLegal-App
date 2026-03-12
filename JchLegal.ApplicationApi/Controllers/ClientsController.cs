using JchLegal.ApplicationApi.Application.Command.Clients;
using JchLegal.ApplicationApi.Application.DTOs;
using JchLegal.ApplicationApi.Application.Query.Clients;
using JchLegal.Domain.SeedWork;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JchLegal.ApplicationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClientsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponse<ClientDto>>> GetClients(
            [FromQuery] string? search,
            [FromQuery] string? type,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _mediator.Send(new GetClientsRequest
            {
                Search = search,
                TypeCode = type,
                Page = page,
                PageSize = pageSize
            });
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ClientDto>> GetClientById(Guid id)
        {
            var result = await _mediator.Send(new GetClientByIdRequest { Id = id });
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpGet("by-user/{userId:long}")]
        public async Task<ActionResult<ClientDto>> GetClientByUserId(long userId)
        {
            var result = await _mediator.Send(new GetClientByUserIdRequest { UserId = userId });
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ClientDto>> CreateClient([FromBody] CreateClientRequest request)
        {
            var result = await _mediator.Send(request);
            return CreatedAtAction(nameof(GetClientById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ClientDto>> UpdateClient(Guid id, [FromBody] UpdateClientRequest request)
        {
            request.Id = id;
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}
