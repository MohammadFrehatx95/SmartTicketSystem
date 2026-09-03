using TicketService.Domain.Enums;

namespace TicketService.Application.DTOs.Ticket
{
    public class CreateTicketRequest
    {
        public long CustomerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set;  } = string.Empty;
        public TicketPriority Priority { get; set; }
    }
}
 