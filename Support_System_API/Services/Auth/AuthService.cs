using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Support_System_API.Data;
using DomainUser = Support_System_API.Domain.Entities.User;
using Support_System_API.Domain.Enums;
using Support_System_API.Dtos;
using Support_System_API.Dtos.Auth;
using Support_System_API.Services.Interfaces;

namespace Support_System_API.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<DomainUser> _passwordHasher;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _logger = logger;
        _context = context;
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<DomainUser>();
    }
        
    public async Task<string> RegisterAsync(RegisterDto dto)
    {
        _logger.LogInformation("Starting user registration for email: {Email}", dto.Email);

        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == dto.Email);

        if (emailExists)
        {
            _logger.LogWarning("Registration failed: Email already exists - {Email}", dto.Email);
            throw new Exception("Email already exists");
        }

        var user = new DomainUser
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            Role = UserRole.User
        };
        
        user.PasswordHash = _passwordHasher
            .HashPassword(user, dto.Password);
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Creating user object for email: {Email}", dto.Email);
        
        return GenerateJwtToken(user);
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        _logger.LogInformation("Login attempt for email: {Email}", dto.Email);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
        {
            _logger.LogWarning("Login failed: user not found for email: {Email}", dto.Email);
            throw new Exception("Invalid credentials");
        }
        
        var result = _passwordHasher
            .VerifyHashedPassword(user, user.PasswordHash, dto.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Login failed: invalid password for userId: {UserId}", user.Id);
            throw new Exception("Invalid credentials");
        }
        
        _logger.LogInformation("Login successful for userId: {UserId}", user.Id);
        
        return GenerateJwtToken(user);
    }
    
    
    private string GenerateJwtToken(DomainUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}