namespace TicketService.Application.DTOs.Agent
{
    public class CreateAgentRequest
    {
        public string Department { get; set; } = string.Empty;
        public int MaxOpenTickets { get; set; }
        public bool IsActive { get; set; }
        public bool IsAvailable { get; set; }
    }
}
