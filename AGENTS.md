# VisionPlatform — Agent 入口（V1.0）

## 范围

仅 `VisionPlatform/**`。禁止修改上级 `VM二次开发` WinForms 工程。

## 栈

.NET **8** x64 · WPF · MVVM · HALCON **20.11** · YOLO ONNX **CPU** · Modbus TCP · TCP 长度前缀 JSON

## 流程

1. 读 `.cursor/rules/00-project-governance.mdc` 与任务相关 rule  
2. 计划 + 文件范围 → 实施 → `scripts/verify-project.ps1`  
3. 验收 ID 见 `docs/acceptance-criteria.md`（PASS/FAIL/BLOCKED）

## 禁止

未批准：真 PLC 写入、下载模型、改 VM 旧工程、伪造测试结果
