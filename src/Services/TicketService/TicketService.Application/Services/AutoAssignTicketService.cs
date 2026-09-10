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

    private async Task<AssignTicketResponse> AssignBestAvailableAsync(long ticketId, AssignmentSource source, CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByIdAsNoTrackingAsync(ticketId);

        if (ticket is null)
            throw new Exception("Ticket not found.");

        if (ticket.Status != TicketStatus.New)
            throw new TicketAssignmentConflictException("Ticket cannot be auto-assigned.");

        var agent = await _agentRepository.GetBestAvailableAgentAsync(ticket.Category);

        if (agent is null)
        {
            var failedAttempt = new AssignmentAttempt
            {
                TicketId = ticketId,
                AgentId = null,
                AttemptStatus = AssignmentAttemptStatus.Failed,
                FailureReason = "No available agent found.",
                CreatedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow
            };

            await _attemptRepository.AddAsync(failedAttempt);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            throw new TicketAssignmentConflictException("No available agent found. Ticket will be retried later.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var assignmentReason = source == AssignmentSource.RetryWorker ? "Assigned by retry worker" : "Auto-assigned to best available agent";

            var affectedRows = await _ticketRepository.TryAssignAsync(ticket.Id, agent.Id, source, assignmentReason, cancellationToken);

            if (affectedRows == 0)
                throw new TicketAssignmentConflictException("Ticket was already assigned or cannot be assigned.");

            var updatedTicket = await _ticketRepository.GetByIdAsync(ticket.Id);

            if (updatedTicket is null)
                throw new Exception("Ticket not found after assignment.");

            var workloadAffectedRows = await _agentRepository.TryIncreamentWorkloadAsync(agent.Id, updatedTicket.AssignedAt!.Value, cancellationToken);

            if (workloadAffectedRows == 0)
                throw new TicketAssignmentConflictException("Selected agent is no longer available.");

            var attempt = new AssignmentAttempt
            {
                TicketId = updatedTicket.Id,
                AgentId = agent.Id,
                AttemptStatus = AssignmentAttemptStatus.Succeeded,
                CreatedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow
            };

            await _attemptRepository.AddAsync(attempt);

            var eventPayload = JsonSerializer.Serialize(new
            {
                TicketId = updatedTicket.Id,
                AgentId = agent.Id,
                AssignedAt = updatedTicket.AssignedAt
            });

            var outboxMessage = new OutboxMessage
            {
                EventKey = $"TicketAssigned:{updatedTicket.Id}:{updatedTicket.AssignmentVersion}",
                EventType = "TicketAssigned",
                Payload = eventPayload,
                CreatedAt = DateTime.UtcNow
            };

            await _outboxRepository.AddAsync(outboxMessage);

            var response = new AssignTicketResponse
            {
                TicketId = updatedTicket.Id,
                AssignedAgentId = agent.Id,
                Status = updatedTicket.Status,
                AssignmentSource = updatedTicket.AssignmentSource!.Value,
                AssignedAt = updatedTicket.AssignedAt!.Value
            };

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return response;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);

            _unitOfWork.ClearTracking();

            var failedAttempt = new AssignmentAttempt
            {
                TicketId = ticketId,
                AgentId = agent.Id,
                AttemptStatus = AssignmentAttemptStatus.Failed,
                FailureReason = "Ticket assignment failed during processing.",
                CreatedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow
            };

            await _attemptRepository.AddAsync(failedAttempt);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);

            throw;
        }
    }

    public Task<AssignTicketResponse> AutoAssignAsync(long ticketId, CancellationToken cancellationToken = default)
    {
        return AssignBestAvailableAsync(ticketId, AssignmentSource.AutoAssignment, cancellationToken);
    }

    public Task<AssignTicketResponse> RetryAssignAsync(long ticketId, CancellationToken cancellationToken = default)
    {
        return AssignBestAvailableAsync(ticketId, AssignmentSource.RetryWorker, cancellationToken);
    }
}