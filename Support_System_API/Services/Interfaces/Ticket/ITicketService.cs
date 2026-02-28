
using Support_System_API.Dtos.Ticket;

namespace Support_System_API.Services.Interfaces.Ticket;

public interface ITicketService
{
    Task CreateTicket(CreateTicketDto request, Guid userId);
    Task<bool> UpdatedTicket(UpdateTicketDto request,  Guid tickerId);
    Task<bool> DeletedTicket(Guid id);
    Task<List<TicketListDto>> ReadListTickets();
}