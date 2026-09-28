using NotificationService.Application.Interfaces.Repositories;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _dbContext;

        public NotificationRepository(NotificationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            await _dbContext.Notifications.AddAsync(notification, cancellationToken);
        }
    }
}