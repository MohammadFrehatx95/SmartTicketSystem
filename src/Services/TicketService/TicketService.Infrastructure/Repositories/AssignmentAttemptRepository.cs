using Microsoft.EntityFrameworkCore;
using TicketService.Application.Interfaces.Repositories;
using TicketService.Domain.Entities;
using TicketService.Domain.Enums;
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

        public async Task<int> CountFailedAttemptsAsync(long ticketId, CancellationToken cancellationToken = default)
        {
           return await _dbContext.AssignmentAttempts.CountAsync(a =>  a.TicketId == ticketId &&
                                                                 a.AttemptStatus == AssignmentAttemptStatus.Failed,
                                                                   cancellationToken);
        }
    }
}
