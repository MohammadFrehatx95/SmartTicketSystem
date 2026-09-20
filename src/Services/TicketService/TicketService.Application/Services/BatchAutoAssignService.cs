using Shared.Application.Exceptions;
using TicketService.Application.DTOs.Ticket;
using TicketService.Application.Interfaces.Repositories;
using TicketService.Application.Interfaces.Services;

namespace TicketService.Application.Services;

public class BatchAutoAssignService : IBatchAutoAssignService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAutoAssignTicketService _autoAssignTicketService;

    public BatchAutoAssignService(ITicketRepository ticketRepository, IAutoAssignTicketService autoAssignTicketService)
    {
        _ticketRepository = ticketRepository;
        _autoAssignTicketService = autoAssignTicketService;
    }

    public async Task<BatchAutoAssignResponse> AssignBatchAsync(CancellationToken cancellationToken = default)
    {
        var tickets = await _ticketRepository.GetNewUnassignedTicketsAsync(cancellationToken);

        var response = new BatchAutoAssignResponse
        {
            TotalTickets = tickets.Count
        };

        foreach (var ticket in tickets)
        {
            try
            {
                var result = await _autoAssignTicketService.AutoAssignAsync(ticket.Id, cancellationToken);

                response.Results.Add(new BatchAutoAssignItemResponse
                {
                    TicketId = result.TicketId,
                    AssignedAgentId = result.AssignedAgentId,
                    Succeeded = true
                });

                response.SucceededCount++;
            }
            catch (Exception ex) when (ex is ConflictException or NotFoundException)
            {
                response.Results.Add(new BatchAutoAssignItemResponse
                {
                    TicketId = ticket.Id,
                    AssignedAgentId = null,
                    Succeeded = false,
                    FailureReason = ex.Message
                });

                response.FailedCount++;
            }
        }

        return response;
    }
}