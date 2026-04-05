using Support_System_API.Domain;
using Support_System_API.Domain.Enums;
using Support_System_API.Dtos.Ticket;
using Support_System_API.Shared;

namespace Support_System_API.Domain.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public TicketStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public User User { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<TicketHistory> Histories { get; set; } = new List<TicketHistory>();
    
    public (Result result, string? activity) UpdateDetails(string? title, string? description, Ticket ticket, Guid userId, string role)
    {
        if (ticket.UserId != userId && role != "Admin")
            return (Result.Fail("You must be a system administrator to perform this action"), null);
        
        if (!string.IsNullOrWhiteSpace(title))
            Title = title;

        if (!string.IsNullOrWhiteSpace(description))
            Description = description;
        
        var activity = $"Ticket updated successfully";

        return (Result.Ok(), activity);

    }
    
    public (Result result, string? activity) UpdateStatus(TicketStatus status, string role)
    {
        if (Status == status)
            return (Result.Fail("Status is already the same"), null);

        if (role != "Admin")
            return (Result.Fail("You must be a system administrator to perform this action"), null);
            
        var oldStatus = Status;
        Status = status;
        
        var activity = $"Status changed from {oldStatus} to {Status}";
        
        return (Result.Ok(), activity);
        
    }
}