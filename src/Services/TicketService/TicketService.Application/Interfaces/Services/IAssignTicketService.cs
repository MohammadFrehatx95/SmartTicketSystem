using TicketService.Application.DTOs.Ticket;

namespace TicketService.Application.Interfaces.Services
{
    public interface IAssignTicketService
    {
        public Task<AssignTicketResponse> AssignAsync(long ticketId, AssignTicketRequest request, string idempotencyKey, string? correlationId, CancellationToken cancellationToken = default);
    }
}
