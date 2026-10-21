namespace VisionPlatform.Runtime.Tests;

using System.Buffers;
using System.Text;
using VisionPlatform.Communication.Tcp;
using Xunit;

public class LengthPrefixFramerTests
{
    [Fact]
    public void TryReadFrame_handles_half_packet()
    {
        var framer = new LengthPrefixJsonFramer();
        var json = Encoding.UTF8.GetBytes("{\"a\":1}");
        var frame = framer.WriteFrame(json);
        var buffer = new ReadOnlySequence<byte>(frame.ToArray().AsMemory(0, 2));
        Assert.False(framer.TryReadFrame(ref buffer, out _));
    }
}
