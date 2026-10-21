namespace VisionPlatform.Core.Inspection;

public sealed class InspectionRequest
{
    public required string TraceId { get; init; }
    public string? Source { get; init; }
}

public sealed class ToolRunTrace
{
    public required string ToolId { get; init; }
    public required string Status { get; init; }
    public string? ErrorCode { get; init; }
    public long ElapsedMs { get; init; }
}

public sealed class InspectionRunResult
{
    public required string RunId { get; init; }
    public required string TraceId { get; init; }
    public bool Ok { get; init; }
    public string? Text { get; init; }
    public string? ErrorCode { get; init; }
    public string? ErrorMessage { get; init; }
    public long ElapsedMs { get; init; }
    public IReadOnlyList<ToolRunTrace> ToolTrace { get; init; } = Array.Empty<ToolRunTrace>();
}
