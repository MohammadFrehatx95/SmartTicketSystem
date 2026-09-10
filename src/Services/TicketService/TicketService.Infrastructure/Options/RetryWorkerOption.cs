namespace TicketService.Infrastructure.Options
{
    public class RetryWorkerOption
    {
        public int IntervalMinutes { get; set; } = 1;
        public int MaxRetryAttempts { get; set; } = 3;
    }
}
