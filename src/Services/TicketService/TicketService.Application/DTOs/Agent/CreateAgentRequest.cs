using System.ComponentModel.DataAnnotations;

namespace TicketService.Application.DTOs.Agent
{
    public class CreateAgentRequest
    {
        [Range(1, long.MaxValue)]
        public long DepartmentId { get; set; }

        [Range(1, long.MaxValue)]
        public long IdentityUserId { get; set; }

        [Range(1, int.MaxValue)]
        public int MaxOpenTickets { get; set; }
    }
}