using TicketService.Application.DTOs.Ticket;
using TicketService.Application.Interfaces.Persistence;
using TicketService.Application.Interfaces.Repositories;
using TicketService.Application.Interfaces.Services;
using TicketService.Domain.Entities;
using TicketService.Domain.Enums;

namespace TicketService.Application.Services
{
    public class CreateTicketService : ICreateTicketService
    {
        private readonly ITicketRepository _ticketRepository;

        private readonly IUnitOfWork _unitOfWork;

        public CreateTicketService(ITicketRepository ticketRepository, IUnitOfWork unitOfWork)
        {
            _ticketRepository = ticketRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateTicketResponse> CreateAsync(CreateTicketRequest request, CancellationToken cancellationToken = default)
        {
            var ticket = new Ticket
            {
                CustomerId = request.CustomerId,
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
                CustomerId = request.CustomerId,
                Title = ticket.Title,
                Category = ticket.Category,
                Priority = ticket.Priority,
                Status = ticket.Status,
                CreatedAt = ticket.CreatedAt
            };
        }
    }
}
