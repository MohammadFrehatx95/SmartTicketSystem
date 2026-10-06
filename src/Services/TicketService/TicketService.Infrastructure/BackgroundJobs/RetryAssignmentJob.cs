using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Shared.Application.Exceptions;
using TicketService.Application.Exceptions;
using TicketService.Application.Interfaces.Repositories;
using TicketService.Application.Interfaces.Services;
using TicketService.Domain.Enums;
using TicketService.Infrastructure.Options;

namespace TicketService.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public class RetryAssignmentJob : IJob
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAutoAssignTicketService _autoAssignTicketService;
    private readonly RetryWorkerOption _options;
    private readonly ILogger<RetryAssignmentJob> _logger;

    public RetryAssignmentJob(ITicketRepository ticketRepository, IAutoAssignTicketService autoAssignTicketService, IOptions<RetryWorkerOption> options, ILogger<RetryAssignmentJob> logger)
    {
        _ticketRepository = ticketRepository;
        _autoAssignTicketService = autoAssignTicketService;
        _options = options.Value;
        _logger = logger;
    }

    public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
    {
        var tickets = await _ticketRepository.GetNewUnassignedTicketsAsync(cancellationToken);

        _logger.LogInformation("Retry Assignment Job started. Eligible Tickets: {TicketCount}", tickets.Count);

        foreach (var ticket in tickets)
        {
            var isAssigned = false;
            string? lastFailureReason = null;
            long? lastAgentId = null;

            for (var attemptNumber = 1; attemptNumber <= _options.MaxAttemptsPerRun; attemptNumber++)
            {
                try
                {
                    var result = await _autoAssignTicketService.RetryAssignAsync(ticket.Id, cancellationToken);

                    _logger.LogInformation("Ticket {TicketId} was assigned to Agent {AgentId} successfully on attempt {AttemptNumber}/{MaxAttempts}.", result.TicketId, result.AssignedAgentId, attemptNumber, _options.MaxAttemptsPerRun);

                    isAssigned = true;
                    break;
                }
                catch (RetryAssignmentFailedException ex)
                {
                    lastFailureReason = ex.Message;
                    lastAgentId = ex.AgentId;

                    _logger.LogWarning("Ticket {TicketId} failed with Agent {AgentId} on attempt {AttemptNumber}/{MaxAttempts}. Reason: {Reason}", ticket.Id, ex.AgentId, attemptNumber, _options.MaxAttemptsPerRun, ex.Message);

                    if (ex.AgentId is null)
                    {
                        _logger.LogWarning("No available agent found for Ticket {TicketId}. Retry stopped until the next Quartz run.", ticket.Id);
                        break;
                    }
                    
                    if (attemptNumber == _options.MaxAttemptsPerRun)
                        _logger.LogWarning("Ticket {TicketId} reached {MaxAttempts} failed attempts. It will be retried on the next Quartz run.", ticket.Id, _options.MaxAttemptsPerRun);
                }
                catch (NotFoundException ex)
                {
                    lastFailureReason = ex.Message;
                    _logger.LogWarning("Ticket {TicketId} was not found. Retry stopped. Reason: {Reason}", ticket.Id, ex.Message);
                    break;
                }
                catch (ConflictException ex)
                {
                    var currentTicket = await _ticketRepository.GetByIdAsNoTrackingAsync(ticket.Id);

                    if (currentTicket is null || currentTicket.Status != TicketStatus.New || currentTicket.AssignedAgentId is not null)
                    {
                        _logger.LogInformation("Ticket {TicketId} is no longer eligible for retry assignment.", ticket.Id);
                        lastFailureReason = null;
                        break;
                    }

                    lastFailureReason = ex.Message;
                    _logger.LogWarning("Ticket {TicketId} retry attempt {AttemptNumber}/{MaxAttempts} failed. Reason: {Reason}", ticket.Id, attemptNumber, _options.MaxAttemptsPerRun, ex.Message);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    lastFailureReason = ex.Message;
                    _logger.LogError(ex, "Unexpected error while retrying Ticket {TicketId}. Retry stopped for this ticket.", ticket.Id);
                    break;
                }
            }

            if (!isAssigned && lastFailureReason is not null)
            {
                await _autoAssignTicketService.RecordFailedAttemptAsync(ticket.Id, lastAgentId, lastFailureReason, cancellationToken);
                _logger.LogInformation("Single failed attempt recorded in database for Ticket {TicketId}.", ticket.Id);
            }
        }

        _logger.LogInformation("Retry Assignment Job finished.");
    }
}