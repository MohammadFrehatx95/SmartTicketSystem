using TicketService.Domain.Entities;

namespace TicketService.Application.Interfaces.Repositories
{
    public interface IAgentRepository
    {
        Task<Agent?> GetByIdAsync(long agentId);
        Task<Agent?> GetBestAvailableAgentAsync(string ticketCategory);

        Task AddAsync(Agent agent);
    }
}
