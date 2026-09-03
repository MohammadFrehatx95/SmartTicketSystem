using System;
using System.Collections.Generic;
using System.Text;
using TicketService.Domain.Enums;

namespace TicketService.Domain.Entities
{
    public class AssignmentAttempt
    {
        public long AttemptId { get; set; }
        public long TicketId { get; set; }
        public long? AgentId { get; set; }
        public AssignmentAttemptStatus AttemptStatus { get; set; }
        public string? FailureReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
