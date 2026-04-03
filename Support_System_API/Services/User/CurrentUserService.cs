using System.Security.Claims;
using Support_System_API.Services.Interfaces.User;

namespace Support_System_API.Services.User;

public class CurrentUserService : ICurrentUserService
{
    public Guid UserId { get; }
    public string Role { get; }
    
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;

        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roleClaim = user?.FindFirst(ClaimTypes.Role)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId) || roleClaim is null)
            throw new UnauthorizedAccessException();

        UserId = userId;
        Role = roleClaim;
    }

}