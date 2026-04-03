
using Support_System_API.Dtos.Ticket;
using Support_System_API.Shared;

namespace Support_System_API.Services.Interfaces.Ticket;

public interface ITicketService
{
    Task CreateTicket(CreateTicketDto request, Guid userId);
    Task<Result<TicketResponseDto>> UpdatedTicket(UpdateTicketDto request, Guid ticketId, Guid userId);
    Task<bool> DeletedTicket(Guid id);
    Task<List<TicketListDto>> GetTicketsAsync(Guid userId, string role);
}