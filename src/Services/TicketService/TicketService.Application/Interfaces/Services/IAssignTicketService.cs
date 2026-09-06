using TicketService.Application.DTOs.Ticket;

namespace TicketService.Application.Interfaces.Services
{
    public interface IAssignTicketService
    {
        Task<AssignTicketResponse> AssignAsync(long ticketId, AssignTicketRequest request, CancellationToken cancellationToken = default);
    }
}
