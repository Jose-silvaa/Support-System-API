using Serilog;

namespace Support_System_API.Infrastructure.Logging;

public class LoggingConfiguration
{
    public static void Configure()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Information)
            .WriteTo.Console()
            .CreateLogger();
    }
}