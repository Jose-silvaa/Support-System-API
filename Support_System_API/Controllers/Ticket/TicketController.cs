using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Support_System_API.Dtos.Ticket;
using Support_System_API.Services.Interfaces;
using Support_System_API.Services.Interfaces.Ticket;

namespace Support_System_API.Controllers.Ticket;

[ApiController]
[Route("api/[controller]")]
public class TicketController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }
    
    /// <summary>
    /// Create a new ticket
    /// </summary>
    /// <param name="request">Ticket creation data</param>
    /// <returns>The newly created ticket</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /tickets
    ///     {
    ///        "title": "Login not working",
    ///        "description": "User cannot log into the system",
    ///        "priority": "High"
    ///     }
    ///
    /// Requires authentication via Bearer token.
    /// </remarks>
    /// <response code="201">Ticket created successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">Unauthorized</response>
    [Authorize(Roles = "User,Admin")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto request)
    {
        await _ticketService.CreateTicket(request);
        return Created();
    }
    
    /// <summary>
    /// Get all tickets for the authenticated user
    /// </summary>
    /// <returns>List of tickets</returns>
    /// <remarks>
    /// Requires authentication via Bearer token.
    /// </remarks>
    /// <response code="200">Returns a list of tickets (can be empty)</response>
    /// <response code="401">Unauthorized</response>
    [Authorize]
    [HttpGet("getAllTickets")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTickets()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        
        if (userIdClaim is null || roleClaim is null)
            return Unauthorized();
        
        if (!Guid.TryParse(userIdClaim.Value, out var userId))
            return BadRequest("Invalid user identifier.");
        
        var role = roleClaim.Value;
        
        var tickets  = await _ticketService.GetTicketsAsync(userId, role);

        return Ok(tickets);
    }
    
    /// <summary>
    /// Update an existing ticket
    /// </summary>
    /// <param name="id">Ticket identifier</param>
    /// <param name="request">Ticket data to update</param>
    /// <returns>Updated ticket</returns>
    /// <remarks>
    /// Requires authentication via Bearer token.
    /// Only the ticket owner or an admin can update it.
    /// </remarks>
    /// <response code="200">Ticket updated successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Ticket not found</response>
    [Authorize(Roles = "Admin, User")]
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateTicket(UpdateTicketDto request, Guid id)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var updated = await _ticketService.UpdatedTicket(request, id, userId);

        if (updated is null)
            return NotFound(new { message = "Ticket not found" });
        
        return Ok(updated);
    }
    
    /// <summary>
    /// Delete a ticket
    /// </summary>
    /// <param name="id">Ticket identifier</param>
    /// <remarks>
    /// Requires authentication via Bearer token.
    /// Only the ticket owner or an admin can delete it.
    /// </remarks>
    /// <response code="204">Ticket deleted successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Ticket not found</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicket(Guid id)
    {
        var deletedTicket = await _ticketService.DeletedTicket(id);

        if(!deletedTicket)
            return NotFound("Ticket not found");
        
        return Ok(new { message = "Ticket deleted successfully" });
    }
    
}