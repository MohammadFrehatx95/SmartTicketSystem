using TicketService.Application.Interfaces.Persistence;

namespace TicketService.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TicketDbContext _dbContext;

        public UnitOfWork(TicketDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
