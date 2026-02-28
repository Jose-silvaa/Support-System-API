using Microsoft.EntityFrameworkCore;
using Support_System_API.Data;
using Support_System_API.Domain.Enums;
using Support_System_API.Dtos.Ticket;
using Support_System_API.Services.Interfaces.Ticket;


namespace Support_System_API.Services.Ticket;

public class TicketService : ITicketService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    
    public TicketService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    
    public async Task CreateTicket(CreateTicketDto request, Guid userId)
    {
        var ticket = new Domain.Ticket
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };
        
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdatedTicket(UpdateTicketDto request, Guid ticketId)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == ticketId);

        if (ticket == null)
            return false;
        
        if (!string.IsNullOrWhiteSpace(request.Title))
            ticket.Title = request.Title;

        if (!string.IsNullOrWhiteSpace(request.Description))
            ticket.Description = request.Description;
        
        ticket.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> DeletedTicket(Guid id)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
        
        if(ticket == null)
            return false;
        
        _context.Tickets.Remove(ticket);
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<TicketListDto>> ReadListTickets()
    {
        return await _context.Tickets
            .Select(t => new TicketListDto
            {
                Id = t.Id,
                Title = t.Title,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
            })
            .ToListAsync();
    }
}