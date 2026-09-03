using System;
using System.Collections.Generic;
using System.Text;

namespace TicketService.Domain.Entities
{
    public class IdempotencyRecord
    {
        public long Id { get; set; }
        public string IdempotencyKey { get; set; } = string.Empty;
        public string RequestHash { get; set; } = string.Empty;
        public string ResponseBody { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

    }
}
