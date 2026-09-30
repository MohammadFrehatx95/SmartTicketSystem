using IdentityService.Application.DTOs.Customers;
using IdentityService.Application.Interfaces.Services;
using MassTransit;
using Shared.Contracts.Customers;

namespace IdentityService.Infrastructure.Broker.Consumers;

public class GetOrCreateCustomerConsumer : IConsumer<GetOrCreateCustomerRequest>
{
    private readonly ICustomerService _customerService;

    public GetOrCreateCustomerConsumer(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task Consume(ConsumeContext<GetOrCreateCustomerRequest> context)
    {
        var request = new CustomerRequest
        {
            FirstName = context.Message.FirstName,
            LastName = context.Message.LastName,
            PhoneNumber = context.Message.PhoneNumber,
            Country = context.Message.Country
        };

        var customer = await _customerService.GetOrCreateAsync(request, context.CancellationToken);

        await context.RespondAsync(new GetOrCreateCustomerResponse
        {
            CustomerId = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber,
            Country = customer.Country
        });
    }
}