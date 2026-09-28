using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Exceptions;
using NotificationService.Application.Interfaces.Persistence;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NotificationDbContext _dbContext;

        public UnitOfWork(NotificationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (IsDuplicateProcessedEvent(ex))
            {
                throw new DuplicateProcessedEventException("This event was already processed.", ex);
            }
        }

        private static bool IsDuplicateProcessedEvent(DbUpdateException exception)
        {
            return exception.InnerException is SqlException sqlException
                   && (sqlException.Number == 2601 || sqlException.Number == 2627)
                   && exception.Entries.Any(x => x.Entity is ProcessedEvent);
        }
    }
}