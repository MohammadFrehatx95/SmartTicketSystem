using TicketService.Application.Interfaces.Repositories;
using TicketService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<OutboxMessage>> GetPendingAsync(int batchSize,CancellationToken cancellationToken = default)
        {
            return await _dbContext.OutboxMessages
                .Where(x => x.ProcessedAt == null)
                .OrderBy(x => x.CreatedAt)
                .Take(batchSize)
                .ToListAsync(cancellationToken);
        }
    }
}
