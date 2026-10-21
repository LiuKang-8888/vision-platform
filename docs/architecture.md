# VisionPlatform 架构概要

## 依赖方向

```text
UI.Wpf → Application → Core ← Modules.Abstractions
              ↓
Infrastructure.* → Core + Modules.Abstractions
```

## 程序集（阶段 1）

| 程序集 | 职责 |
|--------|------|
| Core | 配置 DTO、健康快照、调度请求/结果 |
| Modules.Abstractions | IVisionTool、IToolRegistry |
| Application | 注册表、调度器、健康监控、运行前审核 |
| Infrastructure.Logging | Serilog 宿主扩展 |
| Infrastructure.Tools.BuiltIn | Mock 工具 |
| UI.Wpf | 主界面 ViewModel/View |
| Host | 组合根、appsettings |

## 阶段路线图

| 阶段 | 内容 |
|------|------|
| 1 | 骨架、Mock 工具、调度、看门狗基础（当前） |
| 2 | UI 图像/运行/结果完善 |
| 3 | TCP + SimPlc + Modbus TCP |
| 4 | Halcon + YOLO + OCR |
| 5 | 端到端与部署 |
| 6 | FlowGraph 与插件 SPI |

## TCP 帧（已确认）

4 字节 **大端** 无符号长度 + UTF-8 JSON payload。

## OCR

生产默认 **Halcon**；`IOcrBackend` 预留扩展。
