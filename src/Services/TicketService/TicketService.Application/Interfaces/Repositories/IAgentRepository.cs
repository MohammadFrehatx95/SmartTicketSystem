using TicketService.Domain.Entities;

namespace TicketService.Application.Interfaces.Repositories
{
    public interface IAgentRepository
    {
        Task<Agent?> GetBestAvailableAgentAsync(string ticketCategory);
    }
}
