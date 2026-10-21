namespace VisionPlatform.Infrastructure.Tools.BuiltIn;

using System.Diagnostics;
using VisionPlatform.Core.Tools;

public sealed class MockDelayTool : IVisionTool
{
    public ToolDefinition Definition { get; } = new()
    {
        ToolId = "Mock.Delay",
        DisplayName = "模拟延迟",
        Category = "Mock",
        Parameters =
        [
            new ToolParameterDescriptor { Name = "DelayMs", DataType = "Number", Required = true, DefaultValue = 100 }
        ]
    };

    public ToolValidationResult Validate(ToolInvocation invocation)
    {
        if (!invocation.Parameters.TryGetValue("DelayMs", out var ms))
            return ToolValidationResult.Fail("MISSING_PARAM", "缺少 DelayMs。");
        if (ms is not int and not long and not double)
            return ToolValidationResult.Fail("INVALID_PARAM", "DelayMs 必须为数字。");
        var delay = Convert.ToInt32(ms);
        if (delay < 0 || delay > 60_000)
            return ToolValidationResult.Fail("OUT_OF_RANGE", "DelayMs 范围 0–60000。");
        return ToolValidationResult.Ok();
    }

    public async Task<ToolExecutionResult> ExecuteAsync(ToolInvocation invocation, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        var delayMs = Convert.ToInt32(invocation.Parameters["DelayMs"]!);
        try
        {
            await Task.Delay(delayMs, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            return ToolExecutionResult.Fail(
                ToolExecutionStatus.Cancelled,
                "CANCELLED",
                "已取消。",
                sw.Elapsed);
        }

        sw.Stop();
        return ToolExecutionResult.Success(new Dictionary<string, object?>(), sw.Elapsed);
    }
}
