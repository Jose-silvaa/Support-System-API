using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Support_System_API.Data;
using DomainTicket = Support_System_API.Domain.Entities.Ticket;
using Support_System_API.Domain.Enums;
using Support_System_API.Dtos.Ticket;
using Support_System_API.Infrastructure.Logging;
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
    private readonly ILogger<TicketService> _logger;

    
    public TicketService(AppDbContext context, 
        ITicketHistoryService ticketHistoryService, 
        ICurrentUserService currentUserService, 
        IMemoryCache memoryCache,
        ILogger<TicketService> logger)
    {
        _context = context;
        _ticketHistoryService = ticketHistoryService;
        _currentUserService = currentUserService;
        _memoryCache = memoryCache;
        _logger = logger;
    }
    
    public async Task CreateTicket(CreateTicketDto request, Guid userId)
    {
        _logger.LogInformation("Creating ticket for user {UserId}", userId);
        
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
        {
            _logger.LogWarning("Invalid user id {UserId} while creating ticket", userId);
            throw new Exception("Invalid user id");
        }
        
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task<Result<TicketResponseDto>> UpdatedTicket(UpdateTicketDto request, Guid ticketId, Guid userId, String role)
    {
        _logger.LogInformation("Updating ticket {TicketId} by user {UserId}", ticketId, userId);
        
        var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == ticketId);

        if (ticket == null)
        {
            _logger.LogWarning("Ticket {TicketId} not found", ticketId);
            return Result<TicketResponseDto>.Fail("Ticket not found");
        }
        
        var oldStatus = ticket.Status;
        
        if (request.Status != ticket.Status)
        {
            var (result, activity) = ticket.UpdateStatus(request.Status, _currentUserService.Role);

            if (!result.Success)
            {
                _logger.LogWarning(
                    "Failed to update status of ticket {TicketId}: {Message}", 
                    ticketId, 
                    result.Message
                );
                return Result<TicketResponseDto>.Fail(result.Message);
            }
        
            if (activity != null)
            {
                _logger.LogInformation(
                    "Ticket {TicketId} status changed from {OldStatus} to {NewStatus} by user {UserId}",
                    ticketId,
                    oldStatus,
                    ticket.Status,
                    userId
                );

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
        
        _logger.LogInformation("Ticket {TicketId} updated successfully", ticketId);

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
        _logger.LogInformation("Attempting to delete ticket with id {TicketId}", id);
        
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null)
        {
            _logger.LogWarning("Ticket with id {TicketId} not found", id);
            return false;
        }
        
        _context.Tickets.Remove(ticket);
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Ticket with id {TicketId} successfully deleted", id);
        
        return true;
    }

    public async Task<List<TicketListDto>> GetTicketsAsync(Guid userId, string role)
    {
        List<TicketListDto> tickets;

        var cacheKey = $"tickets_{userId}_{role}";

        if (!_memoryCache.TryGetValue(cacheKey, out tickets))
        {
            _logger.LogInformation("Getting tickets from database - DB");

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
            _logger.LogWarning("Getting tickets from cache - CACHE");

        }
        return tickets;
    }
}