using IdentityService.Application.Interfaces.Persistence;
using IdentityService.Infrastructure.Data;

namespace IdentityService.Infrastructure.Persistence;

public class CustomerUnitOfWork : ICustomerUnitOfWork
{
    private readonly AppDbContext _dbContext;

    public CustomerUnitOfWork(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}