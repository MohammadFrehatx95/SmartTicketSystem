using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketService.Application.DTOs.Ticket;
using TicketService.Application.Interfaces.Services;

namespace TicketService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ICreateTicketService _createTicketService;
        private readonly IAssignTicketService _assignTicketService;
        private readonly IAutoAssignTicketService _autoAssignTicketService;
        private readonly IBatchAutoAssignService _batchAutoAssignService;
        private readonly IGetMyTicketsService _getMyTicketsService;

        public TicketsController(ICreateTicketService createTicketService, IAssignTicketService assignTicketService, IAutoAssignTicketService autoAssignTicketService, IBatchAutoAssignService batchAutoAssignService , IGetMyTicketsService getMyTicketsService)
        {
            _createTicketService = createTicketService;
            _assignTicketService = assignTicketService;
            _autoAssignTicketService = autoAssignTicketService;
            _batchAutoAssignService = batchAutoAssignService;
            _getMyTicketsService = getMyTicketsService;
        }

        [Authorize(Roles = "Agent")]
        [HttpGet("my-tickets")]
        public async Task<IActionResult> GetMyTickets(CancellationToken cancellationToken)
        {
            var identityUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _getMyTicketsService.GetAsync(identityUserId, cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTicketRequest request, CancellationToken cancellationToken) 
        {
            var result = await _createTicketService.CreateAsync(request, cancellationToken);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Supervisor")]
        [HttpPut("{ticketId}/assign")]
        public async Task<IActionResult> Assign(long ticketId, AssignTicketRequest request, [FromHeader(Name = "Idempotency-Key")] string idempotencyKey, CancellationToken cancellationToken)
        {
            var result = await _assignTicketService.AssignAsync(ticketId, request, idempotencyKey, cancellationToken);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Supervisor")]
        [HttpPost("{ticketId}/auto-assign")]
        public async Task<IActionResult> AutoAssign(long ticketId, CancellationToken cancellationToken)
        {
            var result = await _autoAssignTicketService.AutoAssignAsync(ticketId, cancellationToken);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Supervisor")]
        [HttpPost("auto-assign-batch")]
        public async Task<IActionResult> AutoAssignBatch(CancellationToken cancellationToken)
        {
            var result = await _batchAutoAssignService.AssignBatchAsync(cancellationToken);
            return Ok(result);
        }
    }
}