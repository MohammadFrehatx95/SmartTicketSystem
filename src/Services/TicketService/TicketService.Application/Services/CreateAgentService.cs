using TicketService.Application.DTOs.Agent;
using TicketService.Application.Interfaces.Persistence;
using TicketService.Application.Interfaces.Repositories;
using TicketService.Application.Interfaces.Services;
using TicketService.Domain.Entities;

namespace TicketService.Application.Services
{
    public class CreateAgentService : ICreateAgentService
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateAgentService(IAgentRepository agentRepository, IUnitOfWork unitOfWork)
        {
            _agentRepository = agentRepository;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<CreateAgentResponse> CreateAsync(CreateAgentRequest request, CancellationToken cancellationToken)
        {
            var agent = new Agent
            {
                Department = request.Department,
                MaxOpenTickets = request.MaxOpenTickets,
                IsActive = request.IsActive,
                IsAvailable = request.IsAvailable,
                CurrentOpenTickets = 0,
                LastAssignedAt = null
            };

            await _agentRepository.AddAsync(agent);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateAgentResponse
            {
                Id = agent.Id,
                Department = agent.Department,
                CurrentOpenTickets = agent.CurrentOpenTickets,
                MaxOpenTickets = agent.MaxOpenTickets,
                IsActive = agent.IsActive,
                IsAvailable = agent.IsAvailable
            };

        }


    }
}
