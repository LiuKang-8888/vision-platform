# VisionPlatform

自研工业视觉软件平台（C# · .NET 8 · WPF · MVVM）。与仓库内 `VM二次开发` 项目 **无代码依赖**。

## 构建

```powershell
cd VisionPlatform
dotnet restore VisionPlatform.slnx
dotnet build VisionPlatform.slnx -c Release
dotnet test VisionPlatform.slnx -c Release
```

或：`.\scripts\verify-project.ps1`

## 运行

```powershell
dotnet run --project src\VisionPlatform.App\VisionPlatform.App.csproj -c Release
```

## 文档

- [架构](docs/architecture.md)
- [验收标准](docs/acceptance-criteria.md)
- [兼容性矩阵](docs/compatibility-matrix.md)（HALCON **20.11**）

## 技术决策摘要

- HALCON 20.11 · YOLO ONNX **CPU** · OCR Halcon + External 占位 · Modbus TCP（映射待定）· TCP 4 字节大端 + JSON
