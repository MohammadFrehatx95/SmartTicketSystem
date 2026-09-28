using Shared.Application.Exceptions;

namespace TicketService.Application.Exceptions;

public class NoAvailableAgentException : ConflictException
{
    public NoAvailableAgentException(string message) : base(message)
    {
    }
}