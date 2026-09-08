using System;
using System.Collections.Generic;
using System.Text;
using TicketService.Application.DTOs.Ticket;

namespace TicketService.Application.Interfaces.Services
{
    public interface IAutoAssignTicketService
    {
        Task<AssignTicketResponse> AutoAssignAsync(long ticketId, CancellationToken cancellationToken = default);
    }
}
