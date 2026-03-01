using Support_System_API.Domain.Enums;

namespace Support_System_API.Dtos.TicketHistory;

public class CreateTicketHistoryDto
{
    public Guid TicketId { get; set; }

    public TicketActivityType Type { get; set; }

    public string? Description { get; set; }

}