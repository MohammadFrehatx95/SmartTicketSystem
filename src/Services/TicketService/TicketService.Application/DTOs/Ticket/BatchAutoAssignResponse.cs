namespace TicketService.Application.DTOs.Ticket;

public class BatchAutoAssignResponse
{
    public int TotalTickets { get; set; }
    public int SucceededCount { get; set; }
    public int FailedCount { get; set; }
    public List<BatchAutoAssignItemResponse> Results { get; set; } = [];
}