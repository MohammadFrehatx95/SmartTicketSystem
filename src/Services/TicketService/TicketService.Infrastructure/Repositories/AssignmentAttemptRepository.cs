using TicketService.Application.Interfaces.Repositories;
using TicketService.Domain.Entities;
using TicketService.Infrastructure.Data;

namespace TicketService.Infrastructure.Repositories
{
    public class AssignmentAttemptRepository : IAssignmentAttemptRepository
    {
        private readonly TicketDbContext _dbContext;

        public AssignmentAttemptRepository(TicketDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(AssignmentAttempt attempt)
        {
            await _dbContext.AssignmentAttempts.AddAsync(attempt);
        }
    }
}
