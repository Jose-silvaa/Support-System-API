using Support_System_API.Domain.Enums;

namespace Support_System_API.Dtos.Ticket;

public class UpdateTicketDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public TicketStatus? Status { get; set; }
    public DateTime? UpdatedAt { get; set; }

}