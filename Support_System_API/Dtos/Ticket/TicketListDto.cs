using Support_System_API.Domain.Enums;

namespace Support_System_API.Dtos.Ticket;

public class TicketListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public TicketStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}