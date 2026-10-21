namespace VisionPlatform.Runtime.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using VisionPlatform.Core.Configuration;
using VisionPlatform.Core.Inspection;
using VisionPlatform.Infrastructure.Tools.BuiltIn;
using VisionPlatform.Runtime.Health;
using VisionPlatform.Runtime.Inspection;
using VisionPlatform.Runtime.Tools;
using Xunit;

public class InspectionSchedulerTests
{
    [Fact]
    public async Task Enqueue_rejects_when_audit_not_confirmed()
    {
        var registry = new ToolRegistry();
        BuiltInToolRegistration.RegisterAll(registry);
        var audit = new PreRunAuditGate();
        var health = new RuntimeHealthMonitor(
            Options.Create(new WatchdogOptions { RequirePreRunAudit = true }),
            NullLogger<RuntimeHealthMonitor>.Instance);
        await health.StartAsync(CancellationToken.None);

        var scheduler = new InspectionScheduler(
            registry, health, audit,
            Options.Create(new InspectionPipelineOptions()),
            Options.Create(new WatchdogOptions { RequirePreRunAudit = true }),
            NullLogger<InspectionScheduler>.Instance);

        var result = await scheduler.EnqueueAsync(new InspectionRequest { TraceId = "t1" }, CancellationToken.None);
        Assert.False(result.Ok);
        Assert.Equal("AUDIT_REQUIRED", result.ErrorCode);

        await health.StopAsync(CancellationToken.None);
        health.Dispose();
    }
}
