using Support_System_API.Services.Interfaces;
using Support_System_API.Dtos;


namespace Support_System_API.GraphQL;

public class Mutation
{
    public async Task<string> Register(
        RegisterRequest input,
        [Service] IAuthService authService)
    {
        return await authService.RegisterAsync(input);
    }

    public async Task<string> Login(
        LoginRequest input,
        [Service] IAuthService authService)
    {
        return await authService.LoginAsync(input);
    }
}
