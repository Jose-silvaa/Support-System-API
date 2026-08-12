using Microsoft.EntityFrameworkCore;
using Support_System_API.Data;
using Support_System_API.Domain.Enums;
using Support_System_API.Dtos.TicketHistory;
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
    
    
    public void AddActivity(Guid ticketId, string oldValue, string newValue, TicketActivityType type, Guid userId)
    {
        var ticketHistory = new DomainTicketHistory
        {
            TicketId = ticketId,
            OldValue = oldValue,
            NewValue = newValue,
            Type = type,
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };
            
        _context.TicketHistories.Add(ticketHistory);
    }

    public async Task<List<TicketHistoryResponse>> GetTicketHistoryAsync(Guid ticketId)
    {

        return await _context.TicketHistories
            .Where(t => t.TicketId == ticketId)
            .Include(t => t.User)
            .Select(t => new TicketHistoryResponse(
                t.User.Email,
                t.TicketId, 
                t.UserId, 
                t.Type,
                t.OldValue,
                t.NewValue,
                t.CreatedAt
            ))
            .ToListAsync();
    }
}