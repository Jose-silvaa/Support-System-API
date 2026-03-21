using Microsoft.EntityFrameworkCore;
using Support_System_API.Data;
using DomainTicket = Support_System_API.Domain.Entities.Ticket;
using Support_System_API.Domain.Enums;
using Support_System_API.Dtos.Ticket;
using Support_System_API.Services.Interfaces.Ticket;
using Support_System_API.Services.Interfaces.TicketHistory;
using Support_System_API.Shared;


namespace Support_System_API.Services.Ticket;

public class TicketService : ITicketService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ITicketHistoryService _ticketHistoryService;

    
    public TicketService(AppDbContext context, IConfiguration configuration, ITicketHistoryService ticketHistoryService)
    {
        _context = context;
        _configuration = configuration;
        _ticketHistoryService = ticketHistoryService;
    }
    
    public async Task CreateTicket(CreateTicketDto request)
    {
        var ticket = new DomainTicket
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow,
            UserId = request.UserId
            
        };
        
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task<Result<TicketResponseDto>> UpdatedTicket(UpdateTicketDto request, Guid ticketId, Guid userId)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == ticketId);

        if (ticket == null)
            return null;
        
        if (!string.IsNullOrWhiteSpace(request.Title))
            ticket.Title = request.Title;

        if (!string.IsNullOrWhiteSpace(request.Description))
            ticket.Description = request.Description;

        if (request.Status.HasValue)
        {
            var oldStatus = ticket.Status;
            
            var result = ticket.ChangeStatus(request.Status.Value);
            
            if (!result.Success)
                return Result<TicketResponseDto>.Fail(result.Message!);; 
            
            _ticketHistoryService.AddActivity(
                ticket.Id,
                $"Status changed from {oldStatus} to {request.Status.Value}",
                TicketActivityType.StatusChanged,
                userId
            );
        }
        
        ticket.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        
        var response = new TicketResponseDto
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            UserId = ticket.UserId,
            UpdatedAt = ticket.UpdatedAt,
            Status = ticket.Status,
        };
        
        return Result<TicketResponseDto>.Ok(response);
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

    public async Task<List<TicketListDto>> GetTicketsAsync(Guid userId, string role)
    {
        IQueryable<DomainTicket> query = _context.Tickets;

        query = role switch
        {
            "Admin" => query,
            "User" => query.Where(t => t.UserId == userId),
            _ => query.Where(t => false)

        };

        return await query
            .Select(t => new TicketListDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                UserId = t.UserId
            })
            .ToListAsync();
    }
}