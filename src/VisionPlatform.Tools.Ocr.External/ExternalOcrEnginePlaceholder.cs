namespace VisionPlatform.Tools.Ocr.External;

using VisionPlatform.Core.Ocr;

/// <summary>独立 OCR 引擎占位（阶段 4+）。</summary>
public sealed class ExternalOcrEnginePlaceholder : IOcrEngine
{
    public string EngineId => "External";
    public Version EngineVersion => new(0, 0, 1, 0);

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public Task<OcrResult> RecognizeAsync(OcrInput input, CancellationToken cancellationToken) =>
        throw new NotSupportedException("External OCR 尚未实现。");
}
