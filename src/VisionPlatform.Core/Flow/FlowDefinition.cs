namespace VisionPlatform.Core.Flow;

/// <summary>流程编排预留（阶段 6 图形化编辑器）。</summary>
public sealed class FlowDefinition
{
    public int SchemaVersion { get; init; } = 1;
    public required string FlowId { get; init; }
    public IReadOnlyList<FlowNode> Nodes { get; init; } = Array.Empty<FlowNode>();
    public IReadOnlyList<FlowConnection> Connections { get; init; } = Array.Empty<FlowConnection>();
}

public sealed class FlowNode
{
    public required string NodeId { get; init; }
    public required string ToolId { get; init; }
    public IReadOnlyDictionary<string, object?> Parameters { get; init; } =
        new Dictionary<string, object?>();
}

public sealed class FlowConnection
{
    public required string FromNodeId { get; init; }
    public required string FromPort { get; init; }
    public required string ToNodeId { get; init; }
    public required string ToPort { get; init; }
}
