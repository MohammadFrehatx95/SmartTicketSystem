using Microsoft.EntityFrameworkCore;
using TicketService.Application.Interfaces.Repositories;
using TicketService.Domain.Entities;
using TicketService.Infrastructure.Data;

namespace TicketService.Infrastructure.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly TicketDbContext _dbContext;

        public TicketRepository(TicketDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Ticket ticket)
        {
            await _dbContext.Tickets.AddAsync(ticket);
        }

        public async Task<Ticket?> GetByIdAsync(long ticketId)
        {
            return await _dbContext.Tickets.FirstOrDefaultAsync(x => x.Id == ticketId);
        }

        
    }
}
