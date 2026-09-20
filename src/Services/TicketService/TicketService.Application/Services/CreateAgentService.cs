using Shared.Application.Exceptions;
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
        private readonly IDepartmentRepository _departmentRepository;

        public CreateAgentService(IAgentRepository agentRepository, IUnitOfWork unitOfWork, IDepartmentRepository departmentRepository)
        {
            _agentRepository = agentRepository;
            _unitOfWork = unitOfWork;
            _departmentRepository = departmentRepository;
        }
        
        public async Task<CreateAgentResponse> CreateAsync(CreateAgentRequest request, CancellationToken cancellationToken)
        {
            var depExists =  await _departmentRepository.ExistsAsync(request.DepartmentId, cancellationToken);

            if (!depExists)
                throw new NotFoundException("Department not found.");

            var agent = new Agent
            {
                DepartmentId = request.DepartmentId,
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
                DepartmentId = agent.DepartmentId,
                CurrentOpenTickets = agent.CurrentOpenTickets,
                MaxOpenTickets = agent.MaxOpenTickets,
                IsActive = agent.IsActive,
                IsAvailable = agent.IsAvailable
            };

        }


    }
}
