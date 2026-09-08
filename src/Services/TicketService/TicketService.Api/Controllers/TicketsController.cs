using Microsoft.AspNetCore.Mvc;
using TicketService.Application.DTOs.Ticket;
using TicketService.Application.Exceptions;
using TicketService.Application.Interfaces.Services;

namespace TicketService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ICreateTicketService _createTicketService;
        private readonly IAssignTicketService  _assignTicketService;
        private readonly IAutoAssignTicketService _autoAssignTicketService;

        public TicketsController(ICreateTicketService createTicketService, IAssignTicketService assignTicketService, IAutoAssignTicketService autoAssignTicketService)
        {
            _createTicketService = createTicketService;
            _assignTicketService = assignTicketService;
            _autoAssignTicketService = autoAssignTicketService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTicketRequest request , CancellationToken cancellationToken)
        {
            var result = await _createTicketService.CreateAsync(request, cancellationToken);

            return Ok(result);
        }

        [HttpPut("{ticketId}/assign")]
        public async Task<IActionResult> Assign(long ticketId, AssignTicketRequest request , [FromHeader(Name = "Idempotency-Key")] string idempotencyKey, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(idempotencyKey))
                return BadRequest("Idempotency-Key Header is required.");

            try
            {

                var result = await _assignTicketService.AssignAsync(ticketId, request, idempotencyKey, cancellationToken);

                return Ok(result);

            }
            catch (TicketAssignmentConflictException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("{ticketId}/auto-assign")]
        public async Task<IActionResult> AutoAssign(long ticketId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _autoAssignTicketService.AutoAssignAsync(ticketId, cancellationToken);

                return Ok(result);
            }
            catch(TicketAssignmentConflictException ex) 
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

    }
}
