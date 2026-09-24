using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Shared.Application.Broker;
using Shared.Application.Events;
using TicketService.Application.Interfaces.Persistence;
using TicketService.Application.Interfaces.Repositories;
using TicketService.Infrastructure.Options;

namespace TicketService.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public class OutboxPublisherJob : IJob
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OutboxPublisherJob> _logger;
    private readonly OutboxPublisherOptions _options;

    public OutboxPublisherJob(IOutboxRepository outboxRepository, IEventPublisher eventPublisher, IUnitOfWork unitOfWork, ILogger<OutboxPublisherJob> logger, IOptions<OutboxPublisherOptions> options)
    {
        _outboxRepository = outboxRepository;
        _eventPublisher = eventPublisher;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _options = options.Value;
    }

    public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
    {
        var messages = await _outboxRepository.GetPendingAsync(_options.BatchSize, cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                if (message.EventType != nameof(TicketAssignedEvent))
                    throw new InvalidOperationException($"Unsupported event type: {message.EventType}");

                var ticketAssignedEvent = JsonSerializer.Deserialize<TicketAssignedEvent>(message.Payload);

                if (ticketAssignedEvent is null)
                    throw new InvalidOperationException("Invalid TicketAssignedEvent payload.");

                await _eventPublisher.PublishAsync(ticketAssignedEvent, cancellationToken);

                message.ProcessedAt = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Outbox message {EventKey} published successfully.", message.EventKey);
            }
            catch (Exception ex)
            {
                message.RetryCount++;

                await _unitOfWork.SaveChangesAsync(CancellationToken.None);

                _logger.LogError(ex, "Failed to publish outbox message {EventKey}. RetryCount: {RetryCount}", message.EventKey, message.RetryCount);
            }
        }
    }
}