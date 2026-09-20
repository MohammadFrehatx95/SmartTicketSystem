namespace TicketService.Domain.Entities
{
    public class Agent
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsAvailable { get; set; }
        public long DepartmentId { get; set; }
        public Department Department { get; set; } = null!;
        public int CurrentOpenTickets { get; set; }
        public int MaxOpenTickets { get; set; }
        public DateTime? LastAssignedAt { get; set; }
    }
}
