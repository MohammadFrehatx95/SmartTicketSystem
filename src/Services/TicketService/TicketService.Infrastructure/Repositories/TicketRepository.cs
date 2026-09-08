using Microsoft.EntityFrameworkCore;
using TicketService.Application.Interfaces.Repositories;
using TicketService.Domain.Entities;
using TicketService.Domain.Enums;
using TicketService.Infrastructure.Data;

namespace TicketService.Infrastructure.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly TicketDbContext _dbContext;

        public TicketRepository(TicketDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Ticket ticket)
        {
            await _dbContext.Tickets.AddAsync(ticket);
        }

        public async Task<Ticket?> GetByIdAsync(long ticketId)
        {
            return await _dbContext.Tickets.FirstOrDefaultAsync(x => x.Id == ticketId);
        }

        public async Task<int> TryAssignAsync(long ticketId, long agentId, AssignmentSource source, CancellationToken cancellationToken = default)
        {
            var assignedAt = DateTime.UtcNow;

            return await _dbContext.Tickets.Where(t => t.Id == ticketId && t.Status == TicketStatus.New && t.AssignedAgentId == null)
                                           .ExecuteUpdateAsync(setters => setters
                                               .SetProperty(t => t.AssignedAgentId, agentId)
                                               .SetProperty(t => t.Status, TicketStatus.Assigned)
                                               .SetProperty(t => t.AssignedAt, assignedAt)
                                               .SetProperty(t => t.AssignmentSource, source)
                                               .SetProperty(t => t.AssignmentReason, "Manual Assignment")
                                               .SetProperty(
                                                   t => t.AssignmentVersion, t => t.AssignmentVersion + 1
                                               ),
                                               cancellationToken);
        }
    }
}
