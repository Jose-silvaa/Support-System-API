using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Support_System_API.Domain.Enums;
using Support_System_API.Services.Interfaces.User;

namespace Support_System_API.Controllers.Users;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize]
    [HttpGet("assignable-users")]
    public async Task<IActionResult> GetAssignableUsers()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var role = Enum.Parse<UserRole>(User.FindFirst(ClaimTypes.Role)!.Value);

        var users = await _userService.GetAssignableUsersAsync(userId, role);

        return Ok(users);
    }
}