namespace VisionPlatform.Runtime.Tools;

using VisionPlatform.Core.Tools;

public sealed class ToolRegistry : IToolRegistry
{
    private readonly Dictionary<string, IVisionTool> _tools = new(StringComparer.Ordinal);

    public void Register(IVisionTool tool)
    {
        ArgumentNullException.ThrowIfNull(tool);
        var id = tool.Definition.ToolId;
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ToolId 不能为空。", nameof(tool));
        _tools[id] = tool;
    }

    public IVisionTool GetRequired(string toolId)
    {
        if (!_tools.TryGetValue(toolId, out var tool))
            throw new KeyNotFoundException($"未注册工具: {toolId}");
        return tool;
    }

    public bool TryGet(string toolId, out IVisionTool? tool) => _tools.TryGetValue(toolId, out tool);

    public IReadOnlyCollection<ToolDefinition> ListDefinitions() =>
        _tools.Values.Select(t => t.Definition).OrderBy(d => d.ToolId).ToList();
}
