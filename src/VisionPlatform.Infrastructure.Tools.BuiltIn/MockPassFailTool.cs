namespace VisionPlatform.Infrastructure.Tools.BuiltIn;

using System.Diagnostics;
using VisionPlatform.Core.Tools;

public sealed class MockPassFailTool : IVisionTool
{
    public ToolDefinition Definition { get; } = new()
    {
        ToolId = "Mock.PassFail",
        DisplayName = "模拟判定",
        Category = "Mock",
        Parameters =
        [
            new ToolParameterDescriptor { Name = "ForceOk", DataType = "Bool", Required = false, DefaultValue = true }
        ]
    };

    public ToolValidationResult Validate(ToolInvocation invocation)
    {
        if (invocation.Parameters.TryGetValue("ForceOk", out var v) && v is not null and not bool)
            return ToolValidationResult.Fail("INVALID_PARAM", "ForceOk 必须为布尔值。");
        return ToolValidationResult.Ok();
    }

    public async Task<ToolExecutionResult> ExecuteAsync(ToolInvocation invocation, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        await Task.Delay(5, cancellationToken).ConfigureAwait(false);
        sw.Stop();

        var forceOk = true;
        if (invocation.Parameters.TryGetValue("ForceOk", out var v) && v is bool b)
            forceOk = b;

        invocation.Inputs.TryGetValue("Text", out var textObj);
        var text = textObj as string ?? string.Empty;
        if (string.IsNullOrEmpty(text))
            text = "0000000000";

        return ToolExecutionResult.Success(
            new Dictionary<string, object?> { ["Ok"] = forceOk, ["Text"] = text },
            sw.Elapsed);
    }
}
