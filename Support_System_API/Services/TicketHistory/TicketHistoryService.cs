using Support_System_API.Data;
using Support_System_API.Domain.Enums;
using DomainTicketHistory = Support_System_API.Domain.Entities.TicketHistory;

using Support_System_API.Services.Interfaces.TicketHistory;

namespace Support_System_API.Services.TicketHistory;

public class TicketHistoryService : ITicketHistoryService
{
    private readonly AppDbContext _context;
    
    public TicketHistoryService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
    }
    
    
    public void AddActivity(Guid ticketId, string description, TicketActivityType type, Guid userId)
    {
        var ticketHistory = new DomainTicketHistory
        {
            TicketId = ticketId,
            Description = description,
            Type = type,
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };
            
        _context.TicketHistories.Add(ticketHistory);
    }
}