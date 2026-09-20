using Shared.Application.Exceptions;

namespace TicketService.Application.Exceptions;

public class RetryAssignmentFailedException : ConflictException
{
    public long? AgentId { get; }

    public RetryAssignmentFailedException(string message, long? agentId) : base(message)
    {
        AgentId = agentId;
    }
}