namespace TicketService.Infrastructure.Options
{
    public class RetryWorkerOption
    {
        public int IntervalMinutes { get; set; } = 1;
        public int MaxRetryAttempts { get; set; } = 3;
        public int [] RetryDelaysMinutes { get; set; } = new[] {1,2,5};

    }
}
