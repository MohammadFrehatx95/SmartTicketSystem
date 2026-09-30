using IdentityService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Interfaces.Repositiories
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
        Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    }
}
