using TicketService.Domain.Enums;

namespace TicketService.Application.DTOs.Ticket
{
    public class MyTicketResponse
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime? AssignedAt { get; set; }
    }
}