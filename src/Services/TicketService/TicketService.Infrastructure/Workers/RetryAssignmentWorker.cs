using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TicketService.Application.Exceptions;
using TicketService.Application.Interfaces.Repositories;
using TicketService.Application.Interfaces.Services;
using TicketService.Infrastructure.Options;

namespace TicketService.Infrastructure.Workers
{
    public class RetryAssignmentWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly RetryWorkerOption _options;

        private readonly ILogger<RetryAssignmentWorker> _logger;

        public RetryAssignmentWorker(IServiceScopeFactory scopeFactory, IOptions<RetryWorkerOption> options, ILogger<RetryAssignmentWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(_options.IntervalMinutes));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {

                using var scope = _scopeFactory.CreateScope();

                var ticketRepository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();

                var attemptRepository = scope.ServiceProvider.GetRequiredService<IAssignmentAttemptRepository>();

                var autoAssignService = scope.ServiceProvider.GetRequiredService<IAutoAssignTicketService>();

                var tickets = await ticketRepository.GetNewUnassignedTicketsAsync(stoppingToken);

                foreach (var ticket in tickets)
                {
                    var failedCount = await attemptRepository.CountFailedRetryAttemptsAsync(ticket.Id, stoppingToken);

                    if (failedCount >= _options.MaxRetryAttempts)
                        continue;

                    var lastFailedAt = await attemptRepository.GetLastFailedRetryAttemptAtAsync(ticket.Id, stoppingToken);

                    var retryDelayMinutes = _options.RetryDelaysMinutes[failedCount];

                    var nextRetryAt = lastFailedAt.HasValue ? lastFailedAt.Value.AddMinutes(retryDelayMinutes) : ticket.CreatedAt.AddMinutes(retryDelayMinutes);

                    if (DateTime.UtcNow < nextRetryAt)
                        continue;

                    try
                    {
                        var attemptNumber = failedCount + 1;

                        _logger.LogInformation("Retry assignment started for Ticket {TicketId} with Attempt Number {AttemptNumber}",ticket.Id,attemptNumber);

                        await autoAssignService.RetryAssignAsync(ticket.Id, stoppingToken);

                        _logger.LogInformation("Ticket {TicketId} assigned successfully on Retry Attempt {AttemptNumber}",ticket.Id, attemptNumber);
                    }
                    catch (TicketAssignmentConflictException ex)
                    {
                        var attemptNumber = failedCount + 1;

                        if (attemptNumber >= _options.MaxRetryAttempts)
                        {
                            _logger.LogWarning("Ticket {TicketId} failed after {MaxAttempts} retry attempts. Retry stopped.",ticket.Id,_options.MaxRetryAttempts);
                        }
                        else
                        {
                            _logger.LogWarning("Retry Attempt {AttemptNumber} failed for Ticket {TicketId}: {Message}", attemptNumber,ticket.Id, ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,"Unexpected error while retrying Ticket {TicketId}",ticket.Id);
                    }
                }
            }
        }
    }
}
