using MassTransit;
using NotificationService.Application.Interfaces.Services;
using Shared.Application.Events;

namespace NotificationService.Infrastructure.Broker.Consumers
{
    public class TicketAssignedConsumer : IConsumer<TicketAssignedEvent>
    {
        private readonly ITicketAssignedNotificationService _notificationService;

        public TicketAssignedConsumer(ITicketAssignedNotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Consume(ConsumeContext<TicketAssignedEvent> context)
        {
            var message = context.Message;

            await _notificationService.HandleAsync(message.EventId, message.TicketId, message.AgentId, message.RecipientUserId, context.CancellationToken);
        }
    }
}