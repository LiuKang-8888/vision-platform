namespace VisionPlatform.Communication.Tcp;

using System.Buffers;
using System.Buffers.Binary;
using VisionPlatform.Core.Communication;

/// <summary>阶段 3 完整实现；当前提供帧格式骨架。</summary>
public sealed class LengthPrefixJsonFramer : IMessageFramer
{
    public int MaxMessageBytes { get; }

    public LengthPrefixJsonFramer(int maxMessageBytes = 65536) => MaxMessageBytes = maxMessageBytes;

    public bool TryReadFrame(ref ReadOnlySequence<byte> buffer, out ReadOnlyMemory<byte> payload)
    {
        payload = ReadOnlyMemory<byte>.Empty;
        if (buffer.Length < 4)
            return false;

        var lengthBytes = buffer.FirstSpan.Slice(0, Math.Min(4, buffer.FirstSpan.Length));
        if (lengthBytes.Length < 4)
            return false;

        Span<byte> lenBuf = stackalloc byte[4];
        buffer.Slice(0, 4).CopyTo(lenBuf);
        var length = BinaryPrimitives.ReadInt32BigEndian(lenBuf);
        if (length < 0 || length > MaxMessageBytes)
            throw new InvalidOperationException($"消息长度非法: {length}");

        if (buffer.Length < 4 + length)
            return false;

        payload = buffer.Slice(4, length).ToArray();
        buffer = buffer.Slice(4 + length);
        return true;
    }

    public ReadOnlyMemory<byte> WriteFrame(ReadOnlyMemory<byte> payload)
    {
        if (payload.Length > MaxMessageBytes)
            throw new ArgumentOutOfRangeException(nameof(payload));
        var result = new byte[4 + payload.Length];
        BinaryPrimitives.WriteInt32BigEndian(result.AsSpan(0, 4), payload.Length);
        payload.CopyTo(result.AsMemory(4));
        return result;
    }
}
