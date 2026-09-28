using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Application.Events;
using NotificationService.Application.Interfaces.Services;

namespace NotificationService.Infrastructure.Broker.Consumers;

public class TicketAssignedConsumer : IConsumer<TicketAssignedEvent>
{
    private readonly ITicketAssignedNotificationService _notificationService;
    private readonly ILogger<TicketAssignedConsumer> _logger;

    public TicketAssignedConsumer(ITicketAssignedNotificationService notificationService, ILogger<TicketAssignedConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<TicketAssignedEvent> context)
    {
        var message = context.Message;

        var correlationId = string.IsNullOrWhiteSpace(message.CorrelationId) ? Guid.NewGuid().ToString() : message.CorrelationId;

        using var scope = _logger.BeginScope("CorrelationId: {CorrelationId}", correlationId);

        _logger.LogInformation("Processing TicketAssignedEvent {EventId} for Ticket {TicketId}.", message.EventId, message.TicketId);

        await _notificationService.HandleAsync(message.EventId, message.TicketId, message.AgentId, message.RecipientUserId, context.CancellationToken);

        _logger.LogInformation("TicketAssignedEvent {EventId} processed successfully.", message.EventId);
    }
}