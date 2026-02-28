using Support_System_API.Domain.Enums;

namespace Support_System_API.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public ICollection<Ticket> Tickets { get; set; } =  new List<Ticket>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}