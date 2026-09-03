using TicketService.Domain.Entities;

namespace TicketService.Application.Interfaces.Repositories
{
    public interface IIdempotencyRepository
    {
        Task<IdempotencyRecord?> GetByKeyAsync(string idempotencyKey);
        Task AddAsync(IdempotencyRecord record);
    }
}
