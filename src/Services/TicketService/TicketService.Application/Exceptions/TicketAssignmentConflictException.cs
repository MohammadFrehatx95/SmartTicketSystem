using System;
using System.Collections.Generic;
using System.Text;

namespace TicketService.Application.Exceptions
{
    public class TicketAssignmentConflictException : Exception
    {
        public TicketAssignmentConflictException(string message) : base(message) {

        }
    }
}
