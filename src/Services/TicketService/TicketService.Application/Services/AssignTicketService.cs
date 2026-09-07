using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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

        public async Task<AssignTicketResponse> AssignAsync(long ticketId, AssignTicketRequest request, string idempotencyKey, CancellationToken cancellationToken = default)
        {

            var requestData = $"{ticketId}:{request.AgentId}";

            var requestHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(requestData)));

            var existingRecord = await _idempotencyRepository.GetByKeyAsync(idempotencyKey);

            if (existingRecord is not null)
            {
                if (existingRecord.RequestHash != requestHash)
                    throw new Exception("Idempotency-Key was already used with a different request.");

                var cachedResponse = JsonSerializer.Deserialize<AssignTicketResponse>(existingRecord.ResponseBody);

                if (cachedResponse is null)
                    throw new Exception("Invalid cache response.");

                return cachedResponse;
            }

            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket is null)
                throw new Exception("Ticket not found.");

            if (ticket.Status != TicketStatus.New)
                throw new Exception("Ticket cannot be assigned.");

            var agent = await _agentRepository.GetByIdAsync(request.AgentId);

            if (agent is null)
                throw new Exception("Agent not found.");

            if (!agent.IsActive || !agent.IsAvailable)
                throw new Exception("Agent is not available.");

            if (agent.CurrentOpenTickets >= agent.MaxOpenTickets)
                throw new Exception("Agent reached maximum workload.");

            ticket.AssignedAgentId = agent.Id;
            ticket.Status = TicketStatus.Assigned;
            ticket.AssignedAt = DateTime.UtcNow;
            ticket.AssignmentSource = AssignmentSource.Manual;
            ticket.AssignmentReason = "Manual Assignment";
            ticket.AssignmentVersion++;

            agent.CurrentOpenTickets++;
            agent.LastAssignedAt = DateTime.UtcNow;

            var attempt = new AssignmentAttempt
            {
                TicketId = ticket.Id,
                AgentId = agent.Id,
                AttemptStatus = AssignmentAttemptStatus.Succeeded,
                CreatedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow,
            };

            await _attemptRepository.AddAsync(attempt);


            var eventPayLoad = JsonSerializer.Serialize(new
            {
                TicketId = ticket.Id,
                AgentId = agent.Id,
                AssignedAt = ticket.AssignedAt

            });

            var outboxMessage = new OutboxMessage
            {
                EventKey = $"TicketAssigned:{ticket.Id}:{ticket.AssignmentVersion}",
                EventType = "TicketAssigned",
                Payload = eventPayLoad,
                CreatedAt = DateTime.UtcNow
            };

            await _outboxRepository.AddAsync(outboxMessage);

            var response = new AssignTicketResponse
            {
                TicketId = ticket.Id,
                AssignedAgentId = agent.Id,
                Status = ticket.Status,
                AssignmentSource = ticket.AssignmentSource.Value,
                AssignedAt = ticket.AssignedAt.Value
            };

            var idempotencyRecord = new IdempotencyRecord
            {
                IdempotencyKey = idempotencyKey,
                RequestHash = requestHash,
                ResponseBody = JsonSerializer.Serialize(response),
                StatusCode = 200,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
            };

            await _idempotencyRepository.AddAsync(idempotencyRecord);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return response;
        }
    }
}
