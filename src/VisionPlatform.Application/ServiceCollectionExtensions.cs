namespace VisionPlatform.Application;

using Microsoft.Extensions.DependencyInjection;
using VisionPlatform.Runtime;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVisionPlatformApplication(this IServiceCollection services)
    {
        services.AddVisionPlatformRuntime();
        return services;
    }
}
