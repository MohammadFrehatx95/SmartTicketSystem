using Microsoft.EntityFrameworkCore.Storage;
using System.Transactions;
using TicketService.Application.Interfaces.Persistence;

namespace TicketService.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TicketDbContext _dbContext;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(TicketDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public void ClearTracking()
        {
           _dbContext.ChangeTracker.Clear();
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction is null)
                return;

            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();

            _transaction = null;
            
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction is null)
                return;

            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();

            _transaction = null;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
} 
