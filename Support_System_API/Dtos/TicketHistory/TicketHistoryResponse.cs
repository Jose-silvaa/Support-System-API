using Support_System_API.Domain.Enums;

namespace Support_System_API.Dtos.TicketHistory;

public record TicketHistoryResponse(Guid TicketId, Guid UserId, TicketActivityType Type, string? Description);
    
    