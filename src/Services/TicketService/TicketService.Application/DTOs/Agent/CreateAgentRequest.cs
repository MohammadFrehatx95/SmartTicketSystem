namespace TicketService.Application.DTOs.Agent
{
    public class CreateAgentRequest
    {
        public long DepartmentId { get; set; }
        public int MaxOpenTickets { get; set; }
        public bool IsActive { get; set; }
        public bool IsAvailable { get; set; }
    }
}
