namespace VisionPlatform.Core.Tools;

public sealed class ToolInvocation
{
    public required string RunId { get; init; }
    public required string TraceId { get; init; }
    public IReadOnlyDictionary<string, object?> Inputs { get; init; } = new Dictionary<string, object?>();
    public IReadOnlyDictionary<string, object?> Parameters { get; init; } = new Dictionary<string, object?>();
}

public sealed class ToolValidationResult
{
    public bool IsValid { get; init; }
    public string? ErrorCode { get; init; }
    public string? ErrorMessage { get; init; }

    public static ToolValidationResult Ok() => new() { IsValid = true };

    public static ToolValidationResult Fail(string code, string message) =>
        new() { IsValid = false, ErrorCode = code, ErrorMessage = message };
}

public sealed class ToolExecutionResult
{
    public ToolExecutionStatus Status { get; init; }
    public IReadOnlyDictionary<string, object?> Outputs { get; init; } = new Dictionary<string, object?>();
    public string? ErrorCode { get; init; }
    public string? ErrorMessage { get; init; }
    public TimeSpan Elapsed { get; init; }

    public static ToolExecutionResult Success(
        IReadOnlyDictionary<string, object?>? outputs,
        TimeSpan elapsed) =>
        new()
        {
            Status = ToolExecutionStatus.Succeeded,
            Outputs = outputs ?? new Dictionary<string, object?>(),
            Elapsed = elapsed
        };

    public static ToolExecutionResult Fail(
        ToolExecutionStatus status,
        string code,
        string message,
        TimeSpan elapsed) =>
        new()
        {
            Status = status,
            ErrorCode = code,
            ErrorMessage = message,
            Elapsed = elapsed
        };
}

