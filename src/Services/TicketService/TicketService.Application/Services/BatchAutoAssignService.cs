using Shared.Application.Exceptions;
using TicketService.Application.DTOs.Ticket;
using TicketService.Application.Exceptions;
using TicketService.Application.Interfaces.Repositories;
using TicketService.Application.Interfaces.Services;
using TicketService.Domain.Enums;

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
            catch (NoAvailableAgentException)
            {
                response.SkippedTickets++;
                response.NoAgentAvailable++;
            }
            catch (ConflictException)
            {
                response.SkippedTickets++;

                var currentTicket = await _ticketRepository.GetByIdAsNoTrackingAsync(ticket.Id);

                if (currentTicket is null)
                {
                    response.InvalidStatus++;
                    continue;
                }

                if (currentTicket.AssignedAgentId is not null || currentTicket.Status == TicketStatus.Assigned)
                {
                    response.AlreadyAssigned++;
                }
                else if (currentTicket.Status != TicketStatus.New)
                {
                    response.InvalidStatus++;
                }
                else
                {
                    response.NoAgentAvailable++;
                }
            }
        }

        return response;
    }
}