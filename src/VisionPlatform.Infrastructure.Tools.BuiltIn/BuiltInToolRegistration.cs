namespace VisionPlatform.Infrastructure.Tools.BuiltIn;

using VisionPlatform.Core.Tools;

public static class BuiltInToolRegistration
{
    public static void RegisterAll(IToolRegistry registry)
    {
        registry.Register(new MockGrabImageTool());
        registry.Register(new MockPassFailTool());
        registry.Register(new MockDelayTool());
    }
}
