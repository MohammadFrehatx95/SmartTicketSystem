using System;
using System.Collections.Generic;
using System.Text;
using TicketService.Domain.Enums;

namespace TicketService.Application.DTOs.Ticket
{
    public class CreateTicketResponse
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
