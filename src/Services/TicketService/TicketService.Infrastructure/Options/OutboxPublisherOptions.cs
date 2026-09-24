using System;
using System.Collections.Generic;
using System.Text;

namespace TicketService.Infrastructure.Options
{
    public class OutboxPublisherOptions
    {
        public int IntervalSeconds { get; set; } = 10;
        public int BatchSize { get; set; } = 50; 
    }
}
