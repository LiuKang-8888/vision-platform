namespace VisionPlatform.Runtime;

using Microsoft.Extensions.DependencyInjection;
using VisionPlatform.Runtime.Health;
using VisionPlatform.Runtime.Inspection;
using VisionPlatform.Runtime.Tools;
using VisionPlatform.Core.Tools;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVisionPlatformRuntime(this IServiceCollection services)
    {
        services.AddSingleton<IToolRegistry, ToolRegistry>();
        services.AddSingleton<IPreRunAuditGate, PreRunAuditGate>();
        services.AddSingleton<IRuntimeHealthMonitor, RuntimeHealthMonitor>();
        services.AddHostedService(sp => (RuntimeHealthMonitor)sp.GetRequiredService<IRuntimeHealthMonitor>());
        services.AddSingleton<IInspectionScheduler, InspectionScheduler>();
        return services;
    }
}
