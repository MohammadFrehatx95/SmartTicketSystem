using TicketService.Domain.Entities;
using TicketService.Domain.Enums;

namespace TicketService.Application.Interfaces.Repositories
{
    public interface ITicketRepository
    {
        Task<Ticket?> GetByIdAsync(long ticketId);
        Task AddAsync(Ticket ticket);
        Task<int> TryAssignAsync(long ticketId, long agentId, AssignmentSource source, CancellationToken cancellationToken = default);

    }
}
