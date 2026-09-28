using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Interfaces.Repositories;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Persistence.Repositories
{
    public class ProcessedEventRepository : IProcessedEventRepository
    {
        private readonly NotificationDbContext _dbContext;

        public ProcessedEventRepository(NotificationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExistsAsync(string eventId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProcessedEvents.AnyAsync(x => x.EventId == eventId, cancellationToken);
        }

        public async Task AddAsync(ProcessedEvent processedEvent, CancellationToken cancellationToken = default)
        {
            await _dbContext.ProcessedEvents.AddAsync(processedEvent, cancellationToken);
        }
    }
}