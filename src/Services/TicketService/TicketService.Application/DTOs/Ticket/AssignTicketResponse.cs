using TicketService.Domain.Enums;

namespace TicketService.Application.DTOs.Ticket
{
    public class AssignTicketResponse
    {
        public long TicketId { get; set; }
        public long AssignedAgentId { get; set; }
        public TicketStatus Status { get; set; }
        public AssignmentSource AssignmentSource { get; set; }
        public  DateTime AssignedAt { get; set; }
    }
}
