namespace VisionPlatform.Infrastructure.Logging;

using Microsoft.Extensions.Hosting;
using Serilog;

public static class SerilogHostExtensions
{
    public static IHostBuilder UseVisionPlatformSerilog(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    path: Path.Combine(AppContext.BaseDirectory, "logs", "vision-platform-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 14);
        });
    }
}
