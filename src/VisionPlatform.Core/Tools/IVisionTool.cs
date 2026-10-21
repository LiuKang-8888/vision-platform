namespace VisionPlatform.Core.Tools;

public interface IVisionTool
{
    ToolDefinition Definition { get; }

    ToolValidationResult Validate(ToolInvocation invocation);

    Task<ToolExecutionResult> ExecuteAsync(ToolInvocation invocation, CancellationToken cancellationToken);
}

