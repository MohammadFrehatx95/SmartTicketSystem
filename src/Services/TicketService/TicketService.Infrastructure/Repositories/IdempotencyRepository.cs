using Microsoft.EntityFrameworkCore;
using TicketService.Application.Interfaces.Repositories;
using TicketService.Domain.Entities;
using TicketService.Infrastructure.Data;

namespace TicketService.Infrastructure.Repositories
{
    public class IdempotencyRepository : IIdempotencyRepository
    {
        private readonly TicketDbContext _dbContext;

        public IdempotencyRepository(TicketDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IdempotencyRecord?> GetByKeyAsync(string idempotencyKey)
        {
            var now = DateTime.UtcNow;

            return await _dbContext.IdempotencyRecords.FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey && x.ExpiresAt > now);
        }

        public async Task AddAsync(IdempotencyRecord record)
        {
            await _dbContext.IdempotencyRecords
                .Where(x => x.IdempotencyKey == record.IdempotencyKey && x.ExpiresAt <= DateTime.UtcNow)
                .ExecuteDeleteAsync();

            await _dbContext.IdempotencyRecords.AddAsync(record);
        }
    }
}