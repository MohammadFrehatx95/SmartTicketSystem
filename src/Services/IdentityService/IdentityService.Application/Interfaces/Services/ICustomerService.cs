using IdentityService.Application.DTOs.Customers;

namespace IdentityService.Application.Interfaces.Services
{
    public interface ICustomerService
    {
        Task<CustomerResponse> GetOrCreateAsync(CustomerRequest customer, CancellationToken cancellationToken = default);
    }
}
