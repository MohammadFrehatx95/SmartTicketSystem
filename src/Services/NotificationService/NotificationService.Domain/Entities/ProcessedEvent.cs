namespace NotificationService.Domain.Entities
{
    public class ProcessedEvent
    {
        public long Id { get; set; }
        public string EventId { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; }
    }
}