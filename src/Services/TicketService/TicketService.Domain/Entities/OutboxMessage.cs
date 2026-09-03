using System;
using System.Collections.Generic;
using System.Text;

namespace TicketService.Domain.Entities
{
    public class OutboxMessage
    {
        public long Id { get; set; }
        public string EventKey { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string Payload { get; set;  } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public int RetryCount { get; set; }
    }
}
