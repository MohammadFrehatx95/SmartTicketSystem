using TicketService.Application.DTOs.Ticket;

namespace TicketService.Application.Interfaces.Services;

public interface IBatchAutoAssignService
{
    Task<BatchAutoAssignResponse> AssignBatchAsync(CancellationToken cancellationToken = default);
}