using System;
using System.Collections.Generic;
using System.Text;

namespace TicketService.Domain.Entities
{
    public class Department
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Agent> Agents { get; set; } = new List<Agent>();

    }
}
