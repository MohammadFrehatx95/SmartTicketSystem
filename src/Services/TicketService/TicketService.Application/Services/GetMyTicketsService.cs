using Shared.Application.Exceptions;
using TicketService.Application.DTOs.Ticket;
using TicketService.Application.Interfaces.Repositories;
using TicketService.Application.Interfaces.Services;

namespace TicketService.Application.Services
{
    public class GetMyTicketsService : IGetMyTicketsService
    {
        private readonly IAgentRepository _agentRepository;
        private readonly ITicketRepository _ticketRepository;

        public GetMyTicketsService(IAgentRepository agentRepository, ITicketRepository ticketRepository)
        {
            _agentRepository = agentRepository;
            _ticketRepository = ticketRepository;
        }

        public async Task<List<MyTicketResponse>> GetAsync(long identityUserId, CancellationToken cancellationToken = default)
        {
            var agent = await _agentRepository.GetByIdentityUserIdAsync(identityUserId, cancellationToken);

            if (agent is null)
                throw new NotFoundException("Agent profile not found.");

            var tickets = await _ticketRepository.GetAssignedTicketsByAgentIdAsync(agent.Id, cancellationToken);

            return tickets.Select(t => new MyTicketResponse
            {
                Id = t.Id,
                CustomerId = t.CustomerId,
                Title = t.Title,
                Category = t.Category,
                Priority = t.Priority,
                Status = t.Status,
                AssignedAt = t.AssignedAt

            }).ToList();
        }
    }
}