namespace VisionPlatform.App.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VisionPlatform.Core.Health;
using VisionPlatform.Core.Inspection;
using VisionPlatform.Core.Tools;
using VisionPlatform.Runtime.Health;
using VisionPlatform.Runtime.Inspection;

public sealed partial class MainViewModel : ObservableObject, IDisposable
{
    private readonly IInspectionScheduler _scheduler;
    private readonly IRuntimeHealthMonitor _health;
    private readonly IPreRunAuditGate _auditGate;
    private readonly IToolRegistry _tools;

    public MainViewModel(
        IInspectionScheduler scheduler,
        IRuntimeHealthMonitor health,
        IPreRunAuditGate auditGate,
        IToolRegistry tools)
    {
        _scheduler = scheduler;
        _health = health;
        _auditGate = auditGate;
        _tools = tools;
        _health.HealthChanged += OnHealthChanged;
        RefreshHealth();
        foreach (var def in _tools.ListDefinitions())
            RegisteredTools.Add($"{def.ToolId} — {def.DisplayName}");
    }

    public ObservableCollection<string> RegisteredTools { get; } = new();

    [ObservableProperty]
    private string _healthSummary = "—";

    [ObservableProperty]
    private string _lastResultText = "—";

    [ObservableProperty]
    private string _lastResultOk = "—";

    [ObservableProperty]
    private string _lastError = "";

    [ObservableProperty]
    private string _commStatus = "TCP：阶段 3 | PLC(Modbus TCP)：阶段 3";

    [ObservableProperty]
    private bool _auditConfirmed;

    [ObservableProperty]
    private bool _isBusy;

    partial void OnAuditConfirmedChanged(bool value)
    {
        if (value)
            _auditGate.Confirm(Environment.UserName);
        else
            _auditGate.Reset();
    }

    [RelayCommand(CanExecute = nameof(CanRun))]
    private async Task RunOnceAsync()
    {
        IsBusy = true;
        RunOnceCommand.NotifyCanExecuteChanged();
        try
        {
            var request = new InspectionRequest
            {
                TraceId = DateTimeOffset.Now.ToString("yyyyMMddHHmmssfff"),
                Source = "UI"
            };
            var result = await _scheduler.EnqueueAsync(request, CancellationToken.None).ConfigureAwait(true);
            LastResultOk = result.Ok ? "OK" : "NG";
            LastResultText = result.Text ?? "—";
            LastError = result.ErrorMessage ?? result.ErrorCode ?? "";
        }
        finally
        {
            IsBusy = false;
            RunOnceCommand.NotifyCanExecuteChanged();
        }
    }

    private bool CanRun() => !IsBusy;

    private void OnHealthChanged(object? sender, HealthSnapshot e) =>
        System.Windows.Application.Current.Dispatcher.Invoke(RefreshHealth);

    private void RefreshHealth()
    {
        var snap = _health.GetSnapshot();
        HealthSummary = $"{snap.Overall} @ {snap.TimestampUtc:HH:mm:ss}";
    }

    public void Dispose() => _health.HealthChanged -= OnHealthChanged;
}
