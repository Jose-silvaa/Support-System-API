using Support_System_API.Domain.Enums;
using Support_System_API.Dtos.Ticket;
using Support_System_API.Dtos.TicketHistory;

namespace Support_System_API.Services.Interfaces.TicketHistory;

public interface ITicketHistoryService
{
    void AddActivity(Guid ticketId, string description, TicketActivityType type, Guid userId);
}