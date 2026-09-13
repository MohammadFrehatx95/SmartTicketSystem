using TicketService.Domain.Entities;

namespace TicketService.Application.Interfaces.Repositories
{
    public interface IAssignmentAttemptRepository
    {
        Task AddAsync(AssignmentAttempt attempt);
        Task<int> CountFailedRetryAttemptsAsync(long ticketId, CancellationToken cancellationToken = default);
        Task<DateTime?> GetLastFailedRetryAttemptAtAsync(long ticketId, CancellationToken cancellationToken = default);
    }
}
