namespace VisionPlatform.Runtime.Health;

public sealed class PreRunAuditGate : IPreRunAuditGate
{
    private readonly object _lock = new();
    public bool IsConfirmed { get; private set; }
    public string? ConfirmedBy { get; private set; }
    public DateTimeOffset? ConfirmedAtUtc { get; private set; }

    public void Confirm(string operatorName)
    {
        lock (_lock)
        {
            IsConfirmed = true;
            ConfirmedBy = operatorName;
            ConfirmedAtUtc = DateTimeOffset.UtcNow;
        }
    }

    public void Reset()
    {
        lock (_lock)
        {
            IsConfirmed = false;
            ConfirmedBy = null;
            ConfirmedAtUtc = null;
        }
    }
}
