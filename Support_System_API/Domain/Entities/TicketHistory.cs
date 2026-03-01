using System.ComponentModel.DataAnnotations;
using Support_System_API.Domain.Enums;

namespace Support_System_API.Domain.Entities;

public class TicketHistory
{
    public Guid Id { get; set; }
    
    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public TicketActivityType Type { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; }

} 