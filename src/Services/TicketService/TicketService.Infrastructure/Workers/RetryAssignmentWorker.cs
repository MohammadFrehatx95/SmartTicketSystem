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

                var autoAssignService = scope.ServiceProvider.GetRequiredService<IAutoAssignTicketService>();

                var tickets = await ticketRepository.GetNewUnassignedTicketsAsync(stoppingToken);

                foreach (var ticket in tickets)
                {
                    try
                    {
                        _logger.LogInformation("Retry assignment started for Ticket {TicketId}",
                            ticket.Id);

                        await autoAssignService.RetryAssignAsync( ticket.Id, stoppingToken);

                        _logger.LogInformation("Retry assignment succeeded for Ticket {TicketId}", ticket.Id);
                    }
                    catch (TicketAssignmentConflictException ex)
                    {
                        _logger.LogWarning("Retry assignment failed for Ticket {TicketId}: {Message}",  ticket.Id, ex.Message);
                    }
                }
            }
        }
    }
}
