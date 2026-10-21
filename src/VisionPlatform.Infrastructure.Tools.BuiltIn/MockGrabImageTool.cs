namespace VisionPlatform.Infrastructure.Tools.BuiltIn;

using System.Diagnostics;
using VisionPlatform.Core.Tools;

public sealed class MockGrabImageTool : IVisionTool
{
    public ToolDefinition Definition { get; } = new()
    {
        ToolId = "Mock.GrabImage",
        DisplayName = "模拟采图",
        Category = "Mock",
        Ports =
        [
            new ToolPortDescriptor { Name = "ImageHandle", DataType = "String", IsInput = false }
        ]
    };

    public ToolValidationResult Validate(ToolInvocation invocation) => ToolValidationResult.Ok();

    public async Task<ToolExecutionResult> ExecuteAsync(ToolInvocation invocation, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        await Task.Delay(10, cancellationToken).ConfigureAwait(false);
        sw.Stop();
        var handle = $"mock-image-{invocation.RunId}";
        return ToolExecutionResult.Success(
            new Dictionary<string, object?> { ["ImageHandle"] = handle, ["Text"] = "1234567890" },
            sw.Elapsed);
    }
}
