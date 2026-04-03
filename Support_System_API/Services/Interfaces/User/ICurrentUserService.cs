namespace Support_System_API.Services.Interfaces.User;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string Role { get; }
}