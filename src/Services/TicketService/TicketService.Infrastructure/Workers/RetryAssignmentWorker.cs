using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TicketService.Application.Exceptions;
using TicketService.Application.Interfaces.Repositories;
using TicketService.Application.Interfaces.Services;

namespace TicketService.Infrastructure.Workers
{
    public class RetryAssignmentWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public RetryAssignmentWorker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {

                using var scope = _scopeFactory.CreateScope();

                var ticketRepository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();

                var autoAssignService = scope.ServiceProvider.GetRequiredService<IAutoAssignTicketService>();

                var tickets = await ticketRepository.GetNewUnassignedTicketsAsync(stoppingToken);

                foreach(var ticket in  tickets)
                {
                    try
                    {
                        await autoAssignService.RetryAssignAsync(ticket.Id, stoppingToken);
                    }
                    catch(TicketAssignmentConflictException)
                    {

                    }
                }
            }
        }
    }
}
