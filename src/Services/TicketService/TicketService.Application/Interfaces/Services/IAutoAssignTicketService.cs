using TicketService.Application.DTOs.Ticket;

namespace TicketService.Application.Interfaces.Services
{
    public interface IAutoAssignTicketService
    {
        Task<AssignTicketResponse> AutoAssignAsync(long ticketId, string? correlationId, CancellationToken cancellationToken = default);

        Task<AssignTicketResponse> RetryAssignAsync(long ticketId, CancellationToken cancellationToken = default);
    }
}