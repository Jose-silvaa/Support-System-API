using Microsoft.AspNetCore.Mvc;
using Support_System_API.Dtos;
using Support_System_API.Dtos.Auth;
using Support_System_API.Services.Interfaces;

namespace Support_System_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    
    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="dto">User registration</param>
    /// <returns>JWT token</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var token = await _authService.RegisterAsync(dto);
        return Ok(token);
    }

    /// <summary>
    /// Login to the System
    /// </summary>
    /// <param name="dto">User login credentials</param>
    /// <returns>JWT token</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto);
        return Ok(token);
    }
}