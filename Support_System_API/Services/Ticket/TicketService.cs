using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Support_System_API.Data;
using DomainTicket = Support_System_API.Domain.Entities.Ticket;
using Support_System_API.Domain.Enums;
using Support_System_API.Dtos.Ticket;
using Support_System_API.Services.Interfaces.Ticket;
using Support_System_API.Services.Interfaces.TicketHistory;
using Support_System_API.Services.Interfaces.User;
using Support_System_API.Shared;


namespace Support_System_API.Services.Ticket;

public class TicketService : ITicketService
{
    private readonly AppDbContext _context;
    private readonly ITicketHistoryService _ticketHistoryService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMemoryCache _memoryCache;
    
    public TicketService(AppDbContext context, ITicketHistoryService ticketHistoryService, ICurrentUserService currentUserService, IMemoryCache memoryCache)
    {
        _context = context;
        _ticketHistoryService = ticketHistoryService;
        _currentUserService = currentUserService;
        _memoryCache = memoryCache;
    }
    
    public async Task CreateTicket(CreateTicketDto request, Guid userId)
    {
        var ticket = new DomainTicket
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow,
            UserId = userId,
            
        };
        
        if (!_context.Users.Any(u => u.Id == ticket.UserId))
            throw new Exception("Invalid user id");
        
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task<Result<TicketResponseDto>> UpdatedTicket(UpdateTicketDto request, Guid ticketId, Guid userId, String role)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == ticketId);

        if (ticket == null)
            return Result<TicketResponseDto>.Fail("Ticket not found");
        
        var oldStatus = ticket.Status;
        
        if (request.Status != ticket.Status)
        {
            var (result, activity) = ticket.UpdateStatus(request.Status, _currentUserService.Role);
            
            if (!result.Success)
                return Result<TicketResponseDto>.Fail(result.Message);
        
            if (activity != null)
            {
                _ticketHistoryService.AddActivity(
                    ticket.Id,
                    $"Status changed from {oldStatus} to {ticket.Status}",
                    TicketActivityType.StatusChanged,
                    userId
                );
            
                ticket.UpdatedAt = DateTime.UtcNow;
            }
        }
        
        ticket.UpdateDetails(request.Title, request.Description, ticket, userId, _currentUserService.Role);
        
        await _context.SaveChangesAsync();

        return Result<TicketResponseDto>.Ok(new TicketResponseDto
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            UserId = ticket.UserId,
            UpdatedAt = ticket.UpdatedAt,
            Status = ticket.Status,
        });
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
        List<TicketListDto> tickets;

        var cacheKey = $"tickets_{userId}_{role}";

        if (!_memoryCache.TryGetValue(cacheKey, out tickets))
        {
            Console.WriteLine("❌ Buscando do BANCO");

            IQueryable<DomainTicket> query = _context.Tickets;

            tickets = await query
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

            _memoryCache.Set(cacheKey, tickets, TimeSpan.FromMinutes(5));
        }
        else
        {
            Console.WriteLine("✅ Vindo do CACHE");
        }
        return tickets;
    }
}