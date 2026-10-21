# 看门狗设计（VisionPlatform）

## 1. 开发过程看门狗

每个开发任务结束必须自检：

| 检查项 | 要求 |
|--------|------|
| 计划 | 有分析、范围、阶段编号 |
| 变更范围 | 未修改 `VM二次开发/**`（除非新 scope） |
| 测试 | `dotnet test` 或 BLOCKED 原因 |
| 验收 | 映射 `acceptance_criteria.md` ID |
| 未决问题 | 明确列出 |

## 2. 运行时看门狗

### 2.1 原则

- **独立 Timer**（`PeriodicTimer` / 专用线程），不与 `InspectionScheduler` Worker 共用阻塞调用栈。
- 阶段 1–5：**仅健康采集 + Fault 报告 + 拒绝新 Run**；不自动向 PLC 写入放行/动作线圈。
- **不替代**硬件安全回路。

### 2.2 组件

| 组件 | 职责 |
|------|------|
| `RuntimeHealthMonitor` | 周期刷新 `HealthSnapshot` |
| `RunWatchdog` | `RunTimeoutMs` + `CancellationToken` |
| `PreRunAuditGate` | `RequirePreRunAudit` 时未确认则 `CanAcceptRun=false` |

### 2.3 健康状态

`Unknown → Starting → Healthy ⇄ Degraded → Fault`

Fault 时：`IInspectionScheduler` 拒绝新任务，UI 状态栏红色提示。

### 2.4 配置键

见 Host `appsettings.json` → `Watchdog` 节。
