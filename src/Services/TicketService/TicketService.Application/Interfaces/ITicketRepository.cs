using TicketService.Domain.Entities;

namespace TicketService.Application.Interfaces
{
    public interface ITicketRepository
    {
        Task<Ticket?> GetByIdAsync(long ticketId);
    }
}
