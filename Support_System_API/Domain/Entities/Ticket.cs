using Support_System_API.Domain;
using Support_System_API.Domain.Enums;

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


    private static readonly Dictionary<TicketStatus, List<TicketStatus>> _allowedTransitions =
        new()
        {
            { TicketStatus.Open, new() { TicketStatus.InProgress } },
            { TicketStatus.InProgress, new() { TicketStatus.Closed} },
            { TicketStatus.Closed, new() },
        };

    public void ChangeStatus(TicketStatus newStatus)    
    {
        if (!_allowedTransitions.TryGetValue(Status, out var allowed) 
            || !allowed.Contains(newStatus))
            throw new InvalidOperationException(
                $"Invalid transition from {Status} to {newStatus}");

        Status = newStatus;
    }
}