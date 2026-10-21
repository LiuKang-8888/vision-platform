namespace VisionPlatform.Runtime.Inspection;

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VisionPlatform.Runtime.Health;
using VisionPlatform.Core.Configuration;
using VisionPlatform.Core.Inspection;
using VisionPlatform.Core.Tools;

public sealed class InspectionScheduler : IInspectionScheduler
{
    private readonly IToolRegistry _tools;
    private readonly IRuntimeHealthMonitor _health;
    private readonly IPreRunAuditGate _auditGate;
    private readonly InspectionPipelineOptions _pipeline;
    private readonly WatchdogOptions _watchdog;
    private readonly ILogger<InspectionScheduler> _logger;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public InspectionScheduler(
        IToolRegistry tools,
        IRuntimeHealthMonitor health,
        IPreRunAuditGate auditGate,
        IOptions<InspectionPipelineOptions> pipeline,
        IOptions<WatchdogOptions> watchdog,
        ILogger<InspectionScheduler> logger)
    {
        _tools = tools;
        _health = health;
        _auditGate = auditGate;
        _pipeline = pipeline.Value;
        _watchdog = watchdog.Value;
        _logger = logger;
    }

    public bool IsRunning => _gate.CurrentCount == 0;

    public async ValueTask<InspectionRunResult> EnqueueAsync(
        InspectionRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (_watchdog.RequirePreRunAudit && !_auditGate.IsConfirmed)
        {
            return Reject(request, "AUDIT_REQUIRED", "请先完成运行前审核确认。");
        }

        if (!_health.CanAcceptRun())
        {
            return Reject(request, "HEALTH_FAULT", "健康状态不允许运行（Fault）。");
        }

        if (!await _gate.WaitAsync(0, cancellationToken).ConfigureAwait(false))
        {
            return Reject(request, "BUSY", "已有检测任务正在执行。");
        }

        try
        {
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            linkedCts.CancelAfter(_watchdog.RunTimeoutMs);

            var runId = Guid.NewGuid().ToString("N");
            var sw = Stopwatch.StartNew();
            var traces = new List<ToolRunTrace>();
            var outputs = new Dictionary<string, object?>(StringComparer.Ordinal);
            string? text = null;
            var ok = true;
            string? errorCode = null;
            string? errorMessage = null;

            _health.ReportRunStarted(runId, request.TraceId);

            foreach (var toolId in _pipeline.ToolChain)
            {
                linkedCts.Token.ThrowIfCancellationRequested();
                var tool = _tools.GetRequired(toolId);
                var invocation = new ToolInvocation
                {
                    RunId = runId,
                    TraceId = request.TraceId,
                    Inputs = outputs,
                    Parameters = new Dictionary<string, object?>(StringComparer.Ordinal)
                };

                var validation = tool.Validate(invocation);
                if (!validation.IsValid)
                {
                    ok = false;
                    errorCode = validation.ErrorCode ?? "VALIDATION_FAILED";
                    errorMessage = validation.ErrorMessage;
                    traces.Add(new ToolRunTrace
                    {
                        ToolId = toolId,
                        Status = ToolExecutionStatus.Failed.ToString(),
                        ErrorCode = errorCode,
                        ElapsedMs = 0
                    });
                    break;
                }

                ToolExecutionResult result;
                try
                {
                    result = await tool.ExecuteAsync(invocation, linkedCts.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (linkedCts.IsCancellationRequested)
                {
                    ok = false;
                    errorCode = "TIMEOUT";
                    errorMessage = "检测总超时。";
                    traces.Add(new ToolRunTrace
                    {
                        ToolId = toolId,
                        Status = ToolExecutionStatus.TimedOut.ToString(),
                        ErrorCode = errorCode,
                        ElapsedMs = 0
                    });
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "工具 {ToolId} 未处理异常 RunId={RunId}", toolId, runId);
                    ok = false;
                    errorCode = "TOOL_EXCEPTION";
                    errorMessage = ex.Message;
                    traces.Add(new ToolRunTrace
                    {
                        ToolId = toolId,
                        Status = ToolExecutionStatus.Failed.ToString(),
                        ErrorCode = errorCode,
                        ElapsedMs = 0
                    });
                    break;
                }

                traces.Add(new ToolRunTrace
                {
                    ToolId = toolId,
                    Status = result.Status.ToString(),
                    ErrorCode = result.ErrorCode,
                    ElapsedMs = (long)result.Elapsed.TotalMilliseconds
                });

                foreach (var pair in result.Outputs)
                    outputs[pair.Key] = pair.Value;

                if (outputs.TryGetValue("Text", out var t) && t is string s)
                    text = s;
                if (outputs.TryGetValue("Ok", out var o) && o is bool b)
                    ok = b;

                if (result.Status is ToolExecutionStatus.Failed or ToolExecutionStatus.TimedOut or ToolExecutionStatus.Cancelled)
                {
                    ok = false;
                    errorCode = result.ErrorCode ?? result.Status.ToString();
                    errorMessage = result.ErrorMessage;
                    break;
                }
            }

            sw.Stop();
            var runResult = new InspectionRunResult
            {
                RunId = runId,
                TraceId = request.TraceId,
                Ok = ok,
                Text = text,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                ElapsedMs = sw.ElapsedMilliseconds,
                ToolTrace = traces
            };

            _health.ReportRunCompleted(runResult);
            _logger.LogInformation(
                "检测完成 RunId={RunId} Ok={Ok} Text={Text} ElapsedMs={ElapsedMs}",
                runId, ok, text, sw.ElapsedMilliseconds);

            return runResult;
        }
        finally
        {
            _gate.Release();
        }
    }

    private static InspectionRunResult Reject(InspectionRequest request, string code, string message) =>
        new()
        {
            RunId = Guid.NewGuid().ToString("N"),
            TraceId = request.TraceId,
            Ok = false,
            ErrorCode = code,
            ErrorMessage = message,
            ElapsedMs = 0
        };
}
