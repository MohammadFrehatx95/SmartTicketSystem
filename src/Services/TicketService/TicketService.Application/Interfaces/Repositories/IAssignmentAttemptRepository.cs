using TicketService.Domain.Entities;

namespace TicketService.Application.Interfaces.Repositories
{
    public interface IAssignmentAttemptRepository
    {
        Task AddAsync(AssignmentAttempt attempt);
    }
}
