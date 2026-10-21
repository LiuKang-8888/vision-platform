namespace VisionPlatform.Runtime.Inspection;

using VisionPlatform.Core.Inspection;

public interface IInspectionScheduler
{
    bool IsRunning { get; }

    ValueTask<InspectionRunResult> EnqueueAsync(InspectionRequest request, CancellationToken cancellationToken);
}
