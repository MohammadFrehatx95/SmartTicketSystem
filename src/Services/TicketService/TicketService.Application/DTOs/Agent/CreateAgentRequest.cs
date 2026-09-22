namespace TicketService.Application.DTOs.Agent
{
    public class CreateAgentRequest
    {
        public long DepartmentId { get; set; }
        public long IdentityUserId { get; set; }
        public int MaxOpenTickets { get; set; }
    }
}
