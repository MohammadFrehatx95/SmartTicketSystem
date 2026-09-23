using NotificationService.Domain.Entities;

namespace NotificationService.Application.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
    }
}
