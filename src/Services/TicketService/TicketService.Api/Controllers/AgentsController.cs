using Microsoft.AspNetCore.Mvc;
using TicketService.Application.DTOs.Agent;
using TicketService.Application.Interfaces.Services;

namespace TicketService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AgentsController : ControllerBase
    {
        private readonly ICreateAgentService _createAgentService;

        public AgentsController(ICreateAgentService createAgentService)
        {
            _createAgentService = createAgentService;
        }

        [HttpPost]
        
        public async Task<IActionResult> CreateAsync(CreateAgentRequest request, CancellationToken cancellation)
        {
            var result = await _createAgentService.CreateAsync(request, cancellation);

            return Ok(result);
        }
    }
}
