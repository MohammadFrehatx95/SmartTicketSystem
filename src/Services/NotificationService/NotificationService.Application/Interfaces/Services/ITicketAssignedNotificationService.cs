namespace NotificationService.Application.Interfaces.Services
{
    public interface ITicketAssignedNotificationService
    {
        Task HandleAsync(string eventId, long ticketId, long agentId, long recipientUserId, CancellationToken cancellationToken = default);
    }
}