using System.Buffers;

namespace VisionPlatform.Core.Communication;

/// <summary>4 字节大端长度 + UTF-8 JSON 载荷（阶段 3 实现）。</summary>
public interface IMessageFramer
{
    int MaxMessageBytes { get; }
    bool TryReadFrame(ref ReadOnlySequence<byte> buffer, out ReadOnlyMemory<byte> payload);
    ReadOnlyMemory<byte> WriteFrame(ReadOnlyMemory<byte> payload);
}
