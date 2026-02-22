using Microsoft.EntityFrameworkCore;
using Support_System_API.Domain;
using Support_System_API.Models;

namespace Support_System_API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<User> Users{ get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketHistory> TicketHistories { get; set; }

}