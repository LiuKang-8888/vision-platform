namespace VisionPlatform.Tools.Ocr.Halcon;

using VisionPlatform.Core.Ocr;

/// <summary>阶段 4：实现 IOcrEngine，EngineId = Halcon。</summary>
public sealed class HalconOcrEnginePlaceholder : IOcrEngine
{
    public string EngineId => "Halcon";
    public Version EngineVersion => new(20, 11, 0, 0);

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public Task<OcrResult> RecognizeAsync(OcrInput input, CancellationToken cancellationToken) =>
        throw new NotSupportedException("HAL-001：阶段 4 集成前为占位。");
}
