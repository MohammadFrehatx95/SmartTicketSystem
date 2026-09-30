namespace IdentityService.Application.Interfaces.Persistence;

public interface ICustomerUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}