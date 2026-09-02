using TicketService.Domain.Enums;

namespace TicketService.Domain.Entities
{
    public class Ticket
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; }
        public long? AssignedAgentId { get; set; }
        public DateTime? AssignedAt { get; set; }
        public string? AssignmentReason { get; set; }
        public AssignmentSource? AssignmentSource { get; set; }
        public int AssignmentVersion { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
