using Shared.Application.Events;
using Shared.Application.Exceptions;
using System.Text.Json;
using TicketService.Application.DTOs.Ticket;
using TicketService.Application.Exceptions;
using TicketService.Application.Interfaces.Persistence;
using TicketService.Application.Interfaces.Repositories;
using TicketService.Application.Interfaces.Services;
using TicketService.Domain.Entities;
using TicketService.Domain.Enums;

namespace TicketService.Application.Services;

public class AutoAssignTicketService : IAutoAssignTicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAgentRepository _agentRepository;
    private readonly IAssignmentAttemptRepository _attemptRepository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AutoAssignTicketService(ITicketRepository ticketRepository, IAgentRepository agentRepository, IAssignmentAttemptRepository attemptRepository, IOutboxRepository outboxRepository, IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _agentRepository = agentRepository;
        _attemptRepository = attemptRepository;
        _outboxRepository = outboxRepository;
        _unitOfWork = unitOfWork;
    }

    private async Task<AssignTicketResponse> AssignBestAvailableAsync(long ticketId, AssignmentSource source, string? correlationId, CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByIdAsNoTrackingAsync(ticketId);

        if (ticket is null)
            throw new NotFoundException("Ticket not found.");

        if (ticket.Status != TicketStatus.New)
            throw new ConflictException("Ticket cannot be auto-assigned.");

        var agent = await _agentRepository.GetBestAvailableAgentAsync(ticket.Category);

        if (agent is null)
        {
            var failedAttempt = new AssignmentAttempt
            {
                TicketId = ticketId,
                AgentId = null,
                AttemptStatus = AssignmentAttemptStatus.Failed,
                AssignmentSource = source,
                FailureReason = "No available agent found.",
                CreatedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow
            };

            await _attemptRepository.AddAsync(failedAttempt);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (source == AssignmentSource.RetryWorker)
                throw new RetryAssignmentFailedException("No available agent found.", null);

            throw new NoAvailableAgentException("No available agent found. Ticket will be retried later.");
        }

        var assignedAt = DateTime.UtcNow;
        var newVersion = ticket.AssignmentVersion + 1;

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var assignmentReason = source == AssignmentSource.RetryWorker ? "Assigned by retry worker" : "Auto-assigned to best available agent";

            var affectedRows = await _ticketRepository.TryAssignAsync(ticket.Id, agent.Id, source, assignmentReason, assignedAt, cancellationToken);

            if (affectedRows == 0)
                throw new ConflictException("Ticket was already assigned or cannot be assigned.");

            var workloadAffectedRows = await _agentRepository.TryIncreamentWorkloadAsync(agent.Id, assignedAt, cancellationToken);

            if (workloadAffectedRows == 0)
                throw new NoAvailableAgentException("Selected agent is no longer available.");

            var attempt = new AssignmentAttempt
            {
                TicketId = ticket.Id,
                AgentId = agent.Id,
                AttemptStatus = AssignmentAttemptStatus.Succeeded,
                AssignmentSource = source,
                CreatedAt = assignedAt,
                CompletedAt = DateTime.UtcNow
            };

            await _attemptRepository.AddAsync(attempt);

            var eventKey = $"TicketAssigned:{ticket.Id}:{newVersion}";

            var ticketAssignedEvent = new TicketAssignedEvent
            {
                EventId = eventKey,
                TicketId = ticket.Id,
                AgentId = agent.Id,
                RecipientUserId = agent.IdentityUserId,
                AssignmentVersion = newVersion,
                AssignedAt = assignedAt,
                CorrelationId = correlationId
            };

            var outboxMessage = new OutboxMessage
            {
                EventKey = eventKey,
                EventType = nameof(TicketAssignedEvent),
                Payload = JsonSerializer.Serialize(ticketAssignedEvent),
                CreatedAt = DateTime.UtcNow
            };

            await _outboxRepository.AddAsync(outboxMessage);

            var response = new AssignTicketResponse
            {
                TicketId = ticket.Id,
                AssignedAgentId = agent.Id,
                Status = TicketStatus.Assigned,
                AssignmentSource = source,
                AssignedAt = assignedAt
            };

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return response;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);
            _unitOfWork.ClearTracking();

            var failedAttempt = new AssignmentAttempt
            {
                TicketId = ticketId,
                AgentId = agent.Id,
                AttemptStatus = AssignmentAttemptStatus.Failed,
                AssignmentSource = source,
                FailureReason = "Ticket assignment failed during processing.",
                CreatedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow
            };

            await _attemptRepository.AddAsync(failedAttempt);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);

            throw;
        }
    }

    public Task<AssignTicketResponse> AutoAssignAsync(long ticketId, string? correlationId, CancellationToken cancellationToken = default)
    {
        return AssignBestAvailableAsync(ticketId, AssignmentSource.AutoAssignment, correlationId, cancellationToken);
    }

    public Task<AssignTicketResponse> RetryAssignAsync(long ticketId, CancellationToken cancellationToken = default)
    {
        var correlationId = Guid.NewGuid().ToString();
        return AssignBestAvailableAsync(ticketId, AssignmentSource.RetryWorker, correlationId, cancellationToken);
    }
}