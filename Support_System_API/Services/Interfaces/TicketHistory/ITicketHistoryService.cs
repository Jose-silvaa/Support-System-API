using Support_System_API.Domain.Enums;
using Support_System_API.Dtos.TicketHistory;
using DomainTicketHistory = Support_System_API.Domain.Entities.TicketHistory;

namespace Support_System_API.Services.Interfaces.TicketHistory;

public interface ITicketHistoryService
{
    void AddActivity(Guid ticketId, string? oldValue, string? newValue, TicketActivityType type, Guid userId);
    
    Task<List<TicketHistoryResponse>> GetTicketHistoryAsync(Guid ticketId);

    
    
}