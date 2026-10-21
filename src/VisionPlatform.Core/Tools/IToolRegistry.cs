namespace VisionPlatform.Core.Tools;

public interface IToolRegistry
{
    void Register(IVisionTool tool);

    IVisionTool GetRequired(string toolId);

    bool TryGet(string toolId, out IVisionTool? tool);

    IReadOnlyCollection<ToolDefinition> ListDefinitions();
}

