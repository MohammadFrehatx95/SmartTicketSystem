using System;
using System.Collections.Generic;
using System.Text;
using TicketService.Domain.Entities;

namespace TicketService.Application.Interfaces
{
    public interface IAgentRepository
    {
        Task<Agent?> GetBestAvailableAgentAsync(string ticketCategory);
    }
}
