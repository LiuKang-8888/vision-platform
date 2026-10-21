namespace VisionPlatform.Runtime.Health;

using VisionPlatform.Core.Health;
using VisionPlatform.Core.Inspection;

public interface IRuntimeHealthMonitor
{
    HealthSnapshot GetSnapshot();

    bool CanAcceptRun();

    void ReportRunStarted(string runId, string traceId);

    void ReportRunCompleted(InspectionRunResult result);

    void SetSubsystem(string name, HealthState state, string? message);

    event EventHandler<HealthSnapshot>? HealthChanged;
}
