using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Shared.Application.Events;
using Shared.Application.Exceptions;
using TicketService.Application.DTOs.Ticket;
using TicketService.Application.Interfaces.Persistence;
using TicketService.Application.Interfaces.Repositories;
using TicketService.Application.Interfaces.Services;
using TicketService.Domain.Entities;
using TicketService.Domain.Enums;

namespace TicketService.Application.Services
{
    public class AssignTicketService : IAssignTicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IAgentRepository _agentRepository;
        private readonly IAssignmentAttemptRepository _attemptRepository;
        private readonly IOutboxRepository _outboxRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdempotencyRepository _idempotencyRepository;

        public AssignTicketService(ITicketRepository ticketRepository, IAgentRepository agentRepository, IAssignmentAttemptRepository attemptRepository, IOutboxRepository outboxRepository, IUnitOfWork unitOfWork, IIdempotencyRepository idempotencyRepository)
        {
            _ticketRepository = ticketRepository;
            _agentRepository = agentRepository;
            _attemptRepository = attemptRepository;
            _outboxRepository = outboxRepository;
            _unitOfWork = unitOfWork;
            _idempotencyRepository = idempotencyRepository;
        }

        public async Task<AssignTicketResponse> AssignAsync(long ticketId, AssignTicketRequest request, string idempotencyKey, string? correlationId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(idempotencyKey))
                throw new BadRequestException("Idempotency-Key header is required.");

            var requestBody = JsonSerializer.Serialize(new { TicketId = ticketId, AgentId = request.AgentId });

            var requestHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(requestBody)));

            var existingRecord = await _idempotencyRepository.GetByKeyAsync(idempotencyKey);

            if (existingRecord is not null)
            {
                if (existingRecord.RequestHash != requestHash)
                    throw new ConflictException("Idempotency-Key was already used with a different request.");

                var cachedResponse = JsonSerializer.Deserialize<AssignTicketResponse>(existingRecord.ResponseBody);
                if (cachedResponse is null)
                    throw new InvalidOperationException("Invalid cached idempotency response.");

                return cachedResponse;
            }

            var existingTicket = await _ticketRepository.GetByIdAsNoTrackingAsync(ticketId);

            if (existingTicket is null)
                throw new NotFoundException("Ticket not found.");


            if (existingTicket.Status != TicketStatus.New)
                throw new BadRequestException("Ticket must be in New status to be assigned.");

            var agent = await _agentRepository.GetByIdAsync(request.AgentId);
            if (agent is null)
                throw new NotFoundException("Agent not found.");

            if (!agent.IsActive)
                throw new BadRequestException("Agent is not active.");

            var assignedAt = DateTime.UtcNow;
            var newVersion = existingTicket.AssignmentVersion + 1;

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {

                var affectedRows = await _ticketRepository.TryAssignAsync(ticketId, agent.Id, AssignmentSource.Manual, "Manual Assignment", assignedAt, cancellationToken);

                if (affectedRows == 0)
                    throw new ConflictException("Ticket was already assigned or cannot be assigned.");


                var workloadAffectedRows = await _agentRepository.TryIncreamentWorkloadAsync(agent.Id, assignedAt, cancellationToken);

                if (workloadAffectedRows == 0)
                    throw new ConflictException("Agent reached maximum workload or is no longer available.");


                var attempt = new AssignmentAttempt
                {
                    TicketId = ticketId,
                    AgentId = agent.Id,
                    AttemptStatus = AssignmentAttemptStatus.Succeeded,
                    AssignmentSource = AssignmentSource.Manual,
                    CreatedAt = assignedAt,
                    CompletedAt = DateTime.UtcNow
                };
                await _attemptRepository.AddAsync(attempt);

                var eventKey = $"TicketAssigned:{ticketId}:{newVersion}";
                var ticketAssignedEvent = new TicketAssignedEvent
                {
                    EventId = eventKey,
                    TicketId = ticketId,
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
                    TicketId = ticketId,
                    AssignedAgentId = agent.Id,
                    Status = TicketStatus.Assigned,
                    AssignmentSource = AssignmentSource.Manual,
                    AssignedAt = assignedAt
                };

                var idempotencyRecord = new IdempotencyRecord
                {
                    IdempotencyKey = idempotencyKey,
                    RequestBody = requestBody,
                    RequestHash = requestHash,
                    ResponseBody = JsonSerializer.Serialize(response),
                    StatusCode = 200,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                };


                await _idempotencyRepository.AddAsync(idempotencyRecord);

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
                    AssignmentSource = AssignmentSource.Manual,
                    FailureReason = "Ticket assignment failed during processing.",
                    CreatedAt = DateTime.UtcNow,
                    CompletedAt = DateTime.UtcNow
                };

                await _attemptRepository.AddAsync(failedAttempt);
                await _unitOfWork.SaveChangesAsync(CancellationToken.None);

                throw;
            }
        }
    }
}