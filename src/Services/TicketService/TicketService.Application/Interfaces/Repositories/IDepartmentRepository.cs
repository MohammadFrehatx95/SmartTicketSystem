namespace TicketService.Application.Interfaces.Repositories
{
    public interface IDepartmentRepository
    {
        Task<bool> ExistsAsync(long departmentId, CancellationToken cancellationToken);
    }
}
