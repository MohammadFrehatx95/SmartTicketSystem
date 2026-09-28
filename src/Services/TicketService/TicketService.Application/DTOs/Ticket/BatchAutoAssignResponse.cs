namespace TicketService.Application.DTOs.Ticket;

public class BatchAutoAssignResponse
{
    public int CheckedTickets { get; set; }
    public int AssignedTickets { get; set; }
    public int SkippedTickets { get; set; }
    public int NoAgentAvailable { get; set; }
    public int AlreadyAssigned { get; set; }
    public int InvalidStatus { get; set; }
}