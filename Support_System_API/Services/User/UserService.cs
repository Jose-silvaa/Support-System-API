using Microsoft.EntityFrameworkCore;
using Support_System_API.Data;
using Support_System_API.Domain.Enums;
using Support_System_API.Dtos.User;
using Support_System_API.Services.Interfaces.User;

namespace Support_System_API.Services.User;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
    }
    
    public async Task<List<UserDto>> GetAssignableUsersAsync(Guid currentUserId, UserRole currentUserRole)
    {
        if (currentUserRole == UserRole.Admin)
        {
            return await _context.Users
                .Where(u => u.Role == UserRole.Admin)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email
                })
                .ToListAsync();
        }
        
        return await _context.Users
            .Where(u => u.Id == currentUserId)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email
            })
            .ToListAsync();
    }
}