using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Domain.Entities
{
    public class Notification
    {
        public long Id { get; set; }
        public long TicketId { get; set; }
        public long AgentId { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
