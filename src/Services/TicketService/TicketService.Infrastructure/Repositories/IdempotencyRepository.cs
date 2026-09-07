using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
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
            return await _dbContext.IdempotencyRecords.FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey);
        }

        public async Task AddAsync(IdempotencyRecord record)
        {
            await _dbContext.IdempotencyRecords.AddAsync(record);
        }
    }
}
