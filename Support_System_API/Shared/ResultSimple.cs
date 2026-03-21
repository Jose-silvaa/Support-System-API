namespace Support_System_API.Shared;

public class Result
{
    public bool Success { get; set; }
    public string? Message { get; set; }

    public static Result Ok() => new() { Success = true };
    public static Result Fail(string message) => new() { Success = false, Message = message };
}