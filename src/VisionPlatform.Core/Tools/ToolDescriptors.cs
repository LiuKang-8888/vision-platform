namespace VisionPlatform.Core.Tools;

public sealed class ToolParameterDescriptor
{
    public required string Name { get; init; }
    public required string DataType { get; init; }
    public bool Required { get; init; }
    public object? DefaultValue { get; init; }
}

public sealed class ToolPortDescriptor
{
    public required string Name { get; init; }
    public required string DataType { get; init; }
    public bool IsInput { get; init; }
}

public sealed class ToolDefinition
{
    public required string ToolId { get; init; }
    public required string DisplayName { get; init; }
    public required string Category { get; init; }
    public IReadOnlyList<ToolPortDescriptor> Ports { get; init; } = Array.Empty<ToolPortDescriptor>();
    public IReadOnlyList<ToolParameterDescriptor> Parameters { get; init; } = Array.Empty<ToolParameterDescriptor>();
}

