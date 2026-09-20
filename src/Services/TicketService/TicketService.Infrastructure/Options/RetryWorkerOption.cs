namespace TicketService.Infrastructure.Options
{
    public class RetryWorkerOption
    {
        public int IntervalMinutes { get; set; } = 60;
        public int MaxAttemptsPerRun { get; set; } = 5;
    }
}
