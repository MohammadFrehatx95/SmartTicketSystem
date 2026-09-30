using MassTransit;
using Shared.Contracts.Customers;
using TicketService.Application.DTOs.Ticket;
using TicketService.Application.Interfaces.Persistence;
using TicketService.Application.Interfaces.Repositories;
using TicketService.Application.Interfaces.Services;
using TicketService.Domain.Entities;
using TicketService.Domain.Enums;

namespace TicketService.Application.Services;

public class CreateTicketService : ICreateTicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRequestClient<GetOrCreateCustomerRequest> _customerClient;

    public CreateTicketService(ITicketRepository ticketRepository, IUnitOfWork unitOfWork, IRequestClient<GetOrCreateCustomerRequest> customerClient)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _customerClient = customerClient;
    }

    public async Task<CreateTicketResponse> CreateAsync(CreateTicketRequest request, CancellationToken cancellationToken = default)
    {
        var customerResponse = await _customerClient.GetResponse<GetOrCreateCustomerResponse>(
            new GetOrCreateCustomerRequest
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Country = request.Country
            },
            cancellationToken);

        var customerId = customerResponse.Message.CustomerId;

        var ticket = new Ticket
        {
            CustomerId = customerId,
            Title = request.Title,
            Description = request.Description,
            Category = request.Category,
            Priority = request.Priority,
            Status = TicketStatus.New,
            AssignmentVersion = 0,
            CreatedAt = DateTime.UtcNow
        };

        await _ticketRepository.AddAsync(ticket);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateTicketResponse
        {
            Id = ticket.Id,
            CustomerId = customerId,
            Title = ticket.Title,
            Category = ticket.Category,
            Priority = ticket.Priority,
            Status = ticket.Status,
            CreatedAt = ticket.CreatedAt
        };
    }
}