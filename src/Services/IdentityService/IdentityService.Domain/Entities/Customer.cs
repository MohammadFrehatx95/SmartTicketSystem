using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Domain.Entities
{
    public class Customer
    {
        public long Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

    }
}
