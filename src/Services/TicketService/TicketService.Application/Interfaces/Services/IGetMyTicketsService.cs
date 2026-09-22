using TicketService.Application.DTOs.Ticket;

namespace TicketService.Application.Interfaces.Services
{
    public interface IGetMyTicketsService
    {
        Task<List<MyTicketResponse>> GetAsync(long identityUserId, CancellationToken cancellationToken = default);
    }
}