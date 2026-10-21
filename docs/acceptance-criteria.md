# 验收标准（V1.0 编号）

状态：**PASS** | **FAIL** | **BLOCKED**（BLOCKED ≠ PASS）

## 架构 ARCH-*

| ID | 通过标准 | 阶段 | 当前 |
|----|----------|------|------|
| ARCH-001 | Core 无 WPF/Halcon/Comm 引用 | 1 | PASS（构建） |
| ARCH-002 | 新 Mock 工具不改 App 分支 | 1 | PASS |
| ARCH-003 | 工具契约含状态/错误/耗时 | 1 | PASS |
| ARCH-004 | Flow 模型无 UI 依赖 | 1 | PASS（Core.Flow） |

## UI UI-*

| ID | 阶段 | 当前 |
|----|------|------|
| UI-001~003 | 2 | BLOCKED |

## TCP TCP-* / Modbus MOD-*

| ID | 阶段 | 当前 |
|----|------|------|
| TCP-001~003 | 3 | BLOCKED（Framer 单测部分 PASS） |
| MOD-001~004 | 3 | BLOCKED（映射待定） |

## 算法 HAL/YOLO/OCR

| ID | 阶段 | 当前 |
|----|------|------|
| HAL-001 | 4 | BLOCKED（20.11 待集成） |
| YOLO-001 | 4 | BLOCKED（CPU ONNX 待集成） |
| OCR-001/002 | 4 | BLOCKED（占位） |

## 流程 FLOW-* / 看门狗 WD-* / REL / CFG

| ID | 阶段 | 当前 |
|----|------|------|
| FLOW-001~003 | 2–6 | 部分（线性 ToolChain） |
| WD-001~003 | 1–2 | 部分（Timer 已独立） |
| REL-001 | 5 | BLOCKED |
| CFG-001 | 2+ | 部分（SchemaVersion=1） |

证据：`scripts/verify-project.ps1` 输出。
