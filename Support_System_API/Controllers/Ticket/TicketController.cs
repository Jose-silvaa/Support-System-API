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
    
    [Authorize(Roles = "User")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _ticketService.CreateTicket(request, userId);
        return Created();
    }
    
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAllTickets()
    {
        var tickets  = await _ticketService.ReadListTickets();

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