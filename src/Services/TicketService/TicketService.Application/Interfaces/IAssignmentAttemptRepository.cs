using TicketService.Domain.Entities;

namespace TicketService.Application.Interfaces
{
    public interface IAssignmentAttemptRepository
    {
        Task AddAsync(AssignmentAttempt attempt);
    }
}
