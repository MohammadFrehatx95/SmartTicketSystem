using System;
using System.Collections.Generic;
using System.Text;
using TicketService.Domain.Entities;

namespace TicketService.Application.Interfaces.Repositories
{
    public interface IOutboxRepository
    {
        Task AddAsync(OutboxMessage message);

        Task<List<OutboxMessage>> GetPendingAsync(int batchSize,CancellationToken cancellationToken = default);
    }
}
