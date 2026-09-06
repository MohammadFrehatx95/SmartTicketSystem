using Microsoft.EntityFrameworkCore;
using TicketService.Application.Interfaces.Repositories;
using TicketService.Domain.Entities;
using TicketService.Infrastructure.Data;

namespace TicketService.Infrastructure.Repositories
{
    public class AgentRepository : IAgentRepository
    {
        private readonly TicketDbContext _dbContext;

        public AgentRepository(TicketDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Agent?> GetByIdAsync(long agentId)
        {
            return await _dbContext.Agents.FirstOrDefaultAsync(a => a.Id == agentId);
        }

        public async Task<Agent?> GetBestAvailableAgentAsync(string ticketCategory)
        {
            return await _dbContext.Agents.Where(a => a.IsActive && a.IsAvailable &&
                                                 a.Department == ticketCategory &&
                                                 a.CurrentOpenTickets < a.MaxOpenTickets)
                                           .OrderBy(a => a.CurrentOpenTickets)
                                           .ThenBy(a => a.LastAssignedAt)
                                           .FirstOrDefaultAsync();
        }

        public async Task AddAsync(Agent agent)
        {
            await _dbContext.Agents.AddAsync(agent);
        }
    }
}
