namespace VisionPlatform.Runtime.Tests;

using VisionPlatform.Infrastructure.Tools.BuiltIn;
using VisionPlatform.Runtime.Tools;
using Xunit;

public class ToolRegistryTests
{
    [Fact]
    public void BuiltIn_tools_count_at_least_three()
    {
        var registry = new ToolRegistry();
        BuiltInToolRegistration.RegisterAll(registry);
        var defs = registry.ListDefinitions();
        Assert.True(defs.Count >= 3);
    }
}
