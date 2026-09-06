using Microsoft.AspNetCore.Mvc;
using TicketService.Application.DTOs.Ticket;
using TicketService.Application.Interfaces.Services;

namespace TicketService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ICreateTicketService _createTicketService;
        private readonly IAssignTicketService  _assignTicketService;

        public TicketsController(ICreateTicketService createTicketService, IAssignTicketService assignTicketService)
        {
            _createTicketService = createTicketService;
            _assignTicketService = assignTicketService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTicketRequest request , CancellationToken cancellationToken)
        {
            var result = await _createTicketService.CreateAsync(request, cancellationToken);

            return Ok(result);
        }

        [HttpPut("{ticketId}/assign")]
        public async Task<IActionResult> Assign(long ticketId, AssignTicketRequest request , CancellationToken cancellationToken)
        {
            var result = await _assignTicketService.AssignAsync(ticketId, request, cancellationToken);

            return Ok(result);
        }

       
    }
}
