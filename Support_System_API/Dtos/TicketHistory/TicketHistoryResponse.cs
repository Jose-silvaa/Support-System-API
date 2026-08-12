using Support_System_API.Domain.Enums;

namespace Support_System_API.Dtos.TicketHistory;

public record TicketHistoryResponse(string Email, Guid TicketId, Guid UserId, TicketActivityType Type, object? OldValue, object? NewValue, DateTime CreatedAt);
    
    