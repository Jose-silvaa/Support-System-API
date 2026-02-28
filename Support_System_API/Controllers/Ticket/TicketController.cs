using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Support_System_API.Dtos.Ticket;
using Support_System_API.Services.Interfaces;
using Support_System_API.Services.Interfaces.Ticket;

namespace Support_System_API.Controllers.Ticket;

[ApiController]
[Route("[controller]")]
public class TicketController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }
    
    [Authorize(Roles = "User,Admin")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _ticketService.CreateTicket(request, userId);
        return Created();
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetTickets()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return Unauthorized();
        
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        
        if (userIdClaim is null || roleClaim is null)
            return Unauthorized();
        
        if (!Guid.TryParse(userIdClaim.Value, out var userId))
            return BadRequest("Invalid user identifier.");
        
        var role = roleClaim.Value;
        
        var tickets  = await _ticketService.GetTicketsAsync(userId, role);

        if (tickets.Count == 0)
            return NoContent();

        return Ok(tickets);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTicket(UpdateTicketDto request, Guid id)
    {
        var updated = await _ticketService.UpdatedTicket(request, id);

        if (!updated)
            return NotFound(new { message = "Ticket not found" });
        
        return Ok(new { message = "Ticket updated successfully" });
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicket(Guid id)
    {
        var deletedTicket = await _ticketService.DeletedTicket(id);

        if(!deletedTicket)
            return NotFound("Ticket not found");
        
        return Ok(new { message = "Ticket deleted successfully" });
    }
    
}