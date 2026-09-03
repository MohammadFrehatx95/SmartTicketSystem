using TicketService.Application.DTOs.Ticket;

namespace TicketService.Application.Interfaces.IServices
{
    public interface ICreateTicketService
    {
        Task<CreateTicketResponse> CreateAsync(CreateTicketRequest request, CancellationToken cancellationToken = default);
    }
}
