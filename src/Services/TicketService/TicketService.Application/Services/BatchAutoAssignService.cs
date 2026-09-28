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

    public async Task<BatchAutoAssignResponse> AssignBatchAsync(string? correlationId, CancellationToken cancellationToken = default)
    {
        var tickets = await _ticketRepository.GetNewUnassignedTicketsAsync(cancellationToken);

        var response = new BatchAutoAssignResponse
        {
            CheckedTickets = tickets.Count
        };

        foreach (var ticket in tickets)
        {
            try
            {
                await _autoAssignTicketService.AutoAssignAsync(ticket.Id, correlationId, cancellationToken);
                response.AssignedTickets++;
            }
            catch (ConflictException ex)
            {
                response.SkippedTickets++;

                if (ex.Message.Contains("No available agent", StringComparison.OrdinalIgnoreCase))
                {
                    response.NoAgentAvailable++;
                }
                else if (ex.Message.Contains("already assigned", StringComparison.OrdinalIgnoreCase) || ex.Message.Contains("cannot be auto-assigned", StringComparison.OrdinalIgnoreCase))
                {
                    response.AlreadyAssigned++;
                }
                else
                {
                    response.InvalidStatus++;
                }
            }
            catch (Exception)
            {
                response.SkippedTickets++;
                response.InvalidStatus++;
            }
        }

        return response;
    }
}