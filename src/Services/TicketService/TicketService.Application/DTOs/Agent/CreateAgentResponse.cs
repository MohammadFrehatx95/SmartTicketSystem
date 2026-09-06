using System;
using System.Collections.Generic;
using System.Text;

namespace TicketService.Application.DTOs.Agent
{
    public class CreateAgentResponse
    {
        public long Id { get; set; }
        public string Department { get; set; } = string.Empty;
        public int CurrentOpenTickets { get; set; }
        public int MaxOpenTickets { get; set; }
        public bool IsActive { get; set; }
        public bool IsAvailable { get; set; }
    }
}
