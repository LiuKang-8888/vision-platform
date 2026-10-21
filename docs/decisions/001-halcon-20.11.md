# ADR-001：HALCON 20.11 Progress

- **状态：** 已确认（用户 2026-10-21）
- **决策：** 生产视觉与 OCR 主后端使用 **HALCON 20.11**，通过 `VisionPlatform.Tools.Halcon` + HalconDotNet / HDevEngine 集成。
- **约束：** x64 进程；资源在 Infrastructure 释放；Core 不引用 Halcon 类型。
