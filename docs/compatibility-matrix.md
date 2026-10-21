# 兼容性矩阵

## 运行环境

| 组件 | 要求 |
|------|------|
| OS | Windows 10/11 x64 |
| .NET | **8.0** 运行时（产线必装）；SDK 8.0.x 与 10.x 可并存（`global.json` + rollForward） |
| UI | WPF |

## HALCON（已确认）

| 版本 | .NET 8 x64 | 状态 |
|------|------------|------|
| **20.11 Progress** | 以 MVTec 20.11 文档为准 | **用户已确认许可证**；集成见阶段 4（HAL-001） |

## YOLO

| 项 | 值 |
|----|-----|
| 推理 | **ONNX Runtime** |
| EP | **CPU**（用户确认） |
| 模型 | 本地路径配置，禁止仓库/auto 下载 |

## OCR

| EngineId | 阶段 |
|----------|------|
| Halcon | 4 |
| External | 占位（Tools.Ocr.External） |

## Modbus TCP

寄存器映射：**待定**（用户确认往后放）。
