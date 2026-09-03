using TicketService.Domain.Entities;

namespace TicketService.Application.Interfaces.Repositories
{
    public interface ITicketRepository
    {
        Task<Ticket?> GetByIdAsync(long ticketId);
    }
}
