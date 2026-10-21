namespace VisionPlatform.Runtime.Health;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VisionPlatform.Core.Configuration;
using VisionPlatform.Core.Health;
using VisionPlatform.Core.Inspection;

public sealed class RuntimeHealthMonitor : IRuntimeHealthMonitor, IHostedService, IDisposable
{
    private readonly WatchdogOptions _options;
    private readonly ILogger<RuntimeHealthMonitor> _logger;
    private readonly object _lock = new();
    private readonly Dictionary<string, SubsystemHealth> _subsystems = new(StringComparer.Ordinal);
    private PeriodicTimer? _timer;
    private Task? _loopTask;
    private CancellationTokenSource? _cts;
    private string? _lastRunId;
    private DateTimeOffset? _lastRunCompletedUtc;

    public RuntimeHealthMonitor(
        IOptions<WatchdogOptions> options,
        ILogger<RuntimeHealthMonitor> logger)
    {
        _options = options.Value;
        _logger = logger;
        SetSubsystem("Platform", HealthState.Starting, "正在启动");
    }

    public event EventHandler<HealthSnapshot>? HealthChanged;

    public HealthSnapshot GetSnapshot()
    {
        lock (_lock)
        {
            return BuildSnapshotLocked();
        }
    }

    public bool CanAcceptRun()
    {
        var snapshot = GetSnapshot();
        if (snapshot.Overall == HealthState.Fault)
            return false;
        // 运行前审核由 InspectionScheduler 单独返回 AUDIT_REQUIRED
        return true;
    }

    public void ReportRunStarted(string runId, string traceId)
    {
        _lastRunId = runId;
        _logger.LogDebug("Run 开始 RunId={RunId} TraceId={TraceId}", runId, traceId);
    }

    public void ReportRunCompleted(InspectionRunResult result)
    {
        _lastRunCompletedUtc = DateTimeOffset.UtcNow;
        SetSubsystem("Scheduler", HealthState.Healthy, $"末次 Run {result.RunId} Ok={result.Ok}");
    }

    public void SetSubsystem(string name, HealthState state, string? message)
    {
        lock (_lock)
        {
            _subsystems[name] = new SubsystemHealth
            {
                Name = name,
                State = state,
                Message = message,
                LastUpdatedUtc = DateTimeOffset.UtcNow
            };
            var snapshot = BuildSnapshotLocked();
            HealthChanged?.Invoke(this, snapshot);
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        SetSubsystem("Platform", HealthState.Healthy, "运行中");
        SetSubsystem("Scheduler", HealthState.Healthy, "空闲");
        SetSubsystem("Tcp", HealthState.Degraded, "阶段 3 未集成");
        SetSubsystem("Plc", HealthState.Degraded, "阶段 3 未集成");
        SetSubsystem("Halcon", HealthState.Degraded, "阶段 4 未集成");

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_options.HealthTickMs));
        _loopTask = Task.Run(() => MonitorLoopAsync(_cts.Token), CancellationToken.None);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_cts != null)
            await _cts.CancelAsync().ConfigureAwait(false);
        if (_loopTask != null)
        {
            try
            {
                await _loopTask.WaitAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // 关闭过程取消
            }
        }

        SetSubsystem("Platform", HealthState.Stopped, "已停止");
    }

    private async Task MonitorLoopAsync(CancellationToken cancellationToken)
    {
        if (_timer == null)
            return;

        try
        {
            while (await _timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                // 独立 Timer 线程：仅读状态、写日志，不执行检测
                HealthSnapshot snapshot;
                lock (_lock)
                {
                    snapshot = BuildSnapshotLocked();
                }

                if (snapshot.Overall == HealthState.Fault)
                {
                    _logger.LogWarning("健康 Fault: {Message}",
                        string.Join("; ", snapshot.Subsystems.Where(s => s.State == HealthState.Fault).Select(s => s.Message)));
                }
            }
        }
        catch (OperationCanceledException)
        {
            // 正常停止
        }
    }

    private HealthSnapshot BuildSnapshotLocked()
    {
        var list = _subsystems.Values.OrderBy(s => s.Name).ToList();
        var overall = ComputeOverall(list);
        return new HealthSnapshot
        {
            Overall = overall,
            Subsystems = list,
            TimestampUtc = DateTimeOffset.UtcNow
        };
    }

    private static HealthState ComputeOverall(IReadOnlyList<SubsystemHealth> list)
    {
        if (list.Any(s => s.State == HealthState.Fault))
            return HealthState.Fault;
        if (list.Any(s => s.State == HealthState.Starting))
            return HealthState.Starting;
        if (list.Any(s => s.State == HealthState.Degraded))
            return HealthState.Degraded;
        if (list.All(s => s.State == HealthState.Stopped))
            return HealthState.Stopped;
        return HealthState.Healthy;
    }

    public void Dispose()
    {
        _timer?.Dispose();
        _cts?.Dispose();
    }
}
