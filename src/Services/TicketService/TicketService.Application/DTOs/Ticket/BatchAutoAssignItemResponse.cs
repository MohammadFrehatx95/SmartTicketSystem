namespace TicketService.Application.DTOs.Ticket;

public class BatchAutoAssignItemResponse
{
    public long TicketId { get; set; }
    public long? AssignedAgentId { get; set; }
    public bool Succeeded { get; set; }
    public string? FailureReason { get; set; }
}