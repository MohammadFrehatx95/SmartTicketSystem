using IdentityService.Application.DTOs.Customers;
using IdentityService.Application.Interfaces.Persistence;
using IdentityService.Application.Interfaces.Repositiories;
using IdentityService.Application.Interfaces.Services;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerUnitOfWork _unitOfWork;

    public CustomerService(ICustomerRepository customerRepository, ICustomerUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CustomerResponse> GetOrCreateAsync(CustomerRequest request, CancellationToken cancellationToken = default)
    {
        var existingCustomer = await _customerRepository.GetByPhoneNumberAsync(request.PhoneNumber, cancellationToken);

        if (existingCustomer is not null)
        {
            return new CustomerResponse
            {
                Id = existingCustomer.Id,
                FirstName = existingCustomer.FirstName,
                LastName = existingCustomer.LastName,
                PhoneNumber = existingCustomer.PhoneNumber,
                Country = existingCustomer.Country
            };
        }

        var customer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Country = request.Country
        };

        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CustomerResponse
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber,
            Country = customer.Country
        };
    }
}