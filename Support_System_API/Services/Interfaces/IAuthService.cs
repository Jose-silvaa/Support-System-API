using Support_System_API.Dtos;
using Support_System_API.Dtos.Auth;

namespace Support_System_API.Services.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterDto dto);
    Task<string> LoginAsync(LoginDto dto);
}