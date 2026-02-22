using Support_System_API.Domain.Enums;
using Support_System_API.Models;

namespace Support_System_API.Domain;

public class TicketHistory
{
    public Guid Id { get; set; }
    public TicketStatus OldStatus { get; set; }
    public TicketStatus NewStatus { get; set; }
    public DateTime ChangedAt { get; set; }
    
    public Guid TicketId { get; set; }
    public Guid ChangedBy { get; set; }
    
    public Ticket Ticket { get; set; }
    public User User { get; set; }
} 