using Support_System_API.Domain.Enums;
using Support_System_API.Dtos.User;

namespace Support_System_API.Services.Interfaces.User;

public interface IUserService
{
    Task<List<UserDto>> GetAssignableUsersAsync(Guid currentUserId, UserRole currentUserRole);
}