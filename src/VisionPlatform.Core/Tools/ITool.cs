namespace VisionPlatform.Core.Tools;

/// <summary>V1.0 强类型工具契约（与 IVisionTool 并行演进，阶段 2+ 迁移）。</summary>
public interface ITool
{
    string Id { get; }
    string DisplayName { get; }
    ToolDefinition Descriptor { get; }

    ToolValidationResult Validate(ToolInvocation invocation);

    Task<ToolExecutionResult> ExecuteAsync(ToolInvocation invocation, CancellationToken cancellationToken);
}
