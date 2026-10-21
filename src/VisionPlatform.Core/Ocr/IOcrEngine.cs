namespace VisionPlatform.Core.Ocr;

/// <summary>OCR 双引擎契约（Halcon / External 占位）。</summary>
public interface IOcrEngine : IAsyncDisposable
{
    string EngineId { get; }
    Version EngineVersion { get; }

    Task<OcrResult> RecognizeAsync(OcrInput input, CancellationToken cancellationToken);
}

public sealed class OcrInput
{
    public required string ImageHandleId { get; init; }
}

public sealed record OcrResult(
    string Text,
    float Confidence,
    string EngineId,
    string Status);
