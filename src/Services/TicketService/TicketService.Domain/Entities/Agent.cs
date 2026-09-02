namespace TicketService.Domain.Entities
{
    public class Agent
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsAvailable { get; set; }
        public string Department {  get; set; } = string.Empty;
        public int CurrentOpenTickets { get; set; }
        public int MaxOpenTickets { get; set; }
        public DateTime? LastAssignedAt { get; set; }
    }
}
