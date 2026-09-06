using TicketService.Application.Interfaces.Repositories;
using TicketService.Domain.Entities;
using TicketService.Infrastructure.Data;

namespace TicketService.Infrastructure.Repositories
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly TicketDbContext _dbContext;

        public OutboxRepository(TicketDbContext dbContext)
        {
            _dbContext = dbContext; 
        }

        public async Task AddAsync(OutboxMessage message)
        {
            await _dbContext.OutboxMessages.AddAsync(message);  
        }
    }
}
