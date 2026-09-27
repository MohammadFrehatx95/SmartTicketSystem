using TicketService.Application.DTOs.Ticket;

namespace TicketService.Application.Interfaces.Services;

public interface IBatchAutoAssignService
{
    Task<BatchAutoAssignResponse> AssignBatchAsync(string? correlationId, CancellationToken cancellationToken = default);
}