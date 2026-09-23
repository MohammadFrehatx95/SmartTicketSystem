using NotificationService.Domain.Entities;

namespace NotificationService.Application.Interfaces.Repositories
{
    public interface IProcessedEventRepository
    {
        Task<bool> ExistsAsync(string eventId, CancellationToken cancellationToken = default);
        Task AddAsync(ProcessedEvent processedEvent, CancellationToken cancellationToken = default);
    }
}