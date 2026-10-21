namespace VisionPlatform.Core.Health;

public enum HealthState
{
    Unknown,
    Starting,
    Healthy,
    Degraded,
    Fault,
    Stopped
}

public sealed class SubsystemHealth
{
    public required string Name { get; init; }
    public HealthState State { get; init; }
    public string? Message { get; init; }
    public DateTimeOffset LastUpdatedUtc { get; init; }
}

public sealed class HealthSnapshot
{
    public HealthState Overall { get; init; }
    public IReadOnlyList<SubsystemHealth> Subsystems { get; init; } = Array.Empty<SubsystemHealth>();
    public DateTimeOffset TimestampUtc { get; init; }
}
