using Microsoft.AspNetCore.Mvc;
using Support_System_API.Dtos;
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
    /// <param name="request">User registration</param>
    /// <returns>JWT token</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var token = await _authService.RegisterAsync(request);
        return Ok(token);
    }

    /// <summary>
    /// Login to the System
    /// </summary>
    /// <param name="request">User login credentials</param>
    /// <returns>JWT token</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var token = await _authService.LoginAsync(request);
        return Ok(token);
    }
}