using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Support_System_API.Services.Interfaces.TicketHistory;

namespace Support_System_API.Controllers.TicketHistory;

[ApiController]
[Route("api/[controller]")]
public class TicketHistoryController : ControllerBase
{
    private readonly ITicketHistoryService _ticketHistoryService;
    
    public TicketHistoryController(ITicketHistoryService ticketHistoryService)
    {
        _ticketHistoryService = ticketHistoryService;
    }
    
    [Authorize(Roles = "Admin,User")]
    [HttpGet("{id}")]
    public async Task<IActionResult> HistoryTicket(Guid id)
    {
        var historyTicket = await _ticketHistoryService.GetTicketHistoryAsync(id);

        if(!historyTicket.Any())
            return NotFound("No changes found for this ticket.");
        
        return Ok(historyTicket);
    }
    
}