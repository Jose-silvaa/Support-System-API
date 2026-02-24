using Support_System_API.Dtos;

namespace Support_System_API.Services.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task<string> LoginAsync(LoginRequest request);
}