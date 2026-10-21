namespace VisionPlatform.Runtime.Health;

public interface IPreRunAuditGate
{
    bool IsConfirmed { get; }

    void Confirm(string operatorName);

    void Reset();
}
