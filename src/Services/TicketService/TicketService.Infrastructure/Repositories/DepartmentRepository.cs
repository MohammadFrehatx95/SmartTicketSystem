using Microsoft.EntityFrameworkCore;
using TicketService.Application.Interfaces.Repositories;
using TicketService.Infrastructure.Data;

namespace TicketService.Infrastructure.Repositories
{
    public class DepartmentRepository :  IDepartmentRepository
    {
        public readonly TicketDbContext _dbContext;

        public DepartmentRepository(TicketDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExistsAsync(long departmentId, CancellationToken cancellationToken)
        {
            return await _dbContext.Departments.AnyAsync(x => x.Id == departmentId);
        }
    }
}
