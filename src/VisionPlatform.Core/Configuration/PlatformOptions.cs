namespace VisionPlatform.Core.Configuration;

public sealed class PlatformOptions
{
    public const string SectionName = "Platform";
    public int SchemaVersion { get; set; } = 1;
    public string Environment { get; set; } = "Development";
}

public sealed class WatchdogOptions
{
    public const string SectionName = "Watchdog";
    public int HealthTickMs { get; set; } = 2000;
    public int RunTimeoutMs { get; set; } = 5000;
    public bool RequirePreRunAudit { get; set; } = true;
}

public sealed class InspectionPipelineOptions
{
    public const string SectionName = "Inspection";
    /// <summary>按顺序执行的工具 Id 列表（阶段 1 Mock 链）。</summary>
    public string[] ToolChain { get; set; } = { "Mock.GrabImage", "Mock.PassFail" };
}
