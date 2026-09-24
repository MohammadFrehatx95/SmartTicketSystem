namespace Shared.Application.Events
{
    public class TicketAssignedEvent
    {
        public string EventId { get; set; } = string.Empty;
        public long TicketId { get; set; }
        public long AgentId { get; set; }
        public long RecipientUserId { get; set; }
        public int AssignmentVersion { get; set; }
        public DateTime AssignedAt { get; set; }
        public string? CorrelationId { get; set; }
    }
}