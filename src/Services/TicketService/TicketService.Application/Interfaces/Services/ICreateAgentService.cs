using TicketService.Application.DTOs.Agent;

namespace TicketService.Application.Interfaces.Services
{
    public interface ICreateAgentService
    {
        Task<CreateAgentResponse> CreateAsync(CreateAgentRequest request, CancellationToken cancellationToken = default);
    }
}
      