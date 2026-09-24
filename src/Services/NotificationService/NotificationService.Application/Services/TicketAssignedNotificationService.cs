using NotificationService.Application.Interfaces.Persistence;
using NotificationService.Application.Interfaces.Repositories;
using NotificationService.Application.Interfaces.Services;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Services
{
    public class TicketAssignedNotificationService : ITicketAssignedNotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IProcessedEventRepository _processedEventRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TicketAssignedNotificationService(INotificationRepository notificationRepository, IProcessedEventRepository processedEventRepository, IUnitOfWork unitOfWork)
        {
            _notificationRepository = notificationRepository;
            _processedEventRepository = processedEventRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(string eventId, long ticketId, long agentId, long recipientUserId, CancellationToken cancellationToken = default)
        {
            var alreadyProcessed = await _processedEventRepository.ExistsAsync(eventId, cancellationToken);

            if (alreadyProcessed)
                return;

            var notification = new Notification
            {
                TicketId = ticketId,
                AgentId = agentId,
                RecipientUserId = recipientUserId,
                Message = $"Ticket {ticketId} was assigned to you.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            var processedEvent = new ProcessedEvent
            {
                EventId = eventId,
                ProcessedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification, cancellationToken);
            await _processedEventRepository.AddAsync(processedEvent, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}