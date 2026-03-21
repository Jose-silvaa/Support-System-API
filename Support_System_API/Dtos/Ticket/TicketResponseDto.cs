using Support_System_API.Domain.Enums;

namespace Support_System_API.Dtos.Ticket;

public class TicketResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public TicketStatus Status { get; set; } =  TicketStatus.InProgress;
}