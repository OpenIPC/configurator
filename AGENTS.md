# Agent Guidelines

This repository maintains an incremental plan for delivering Linux support for the OpenIPC Configurator. Follow these instructions when making changes anywhere in this repo.

## Planning & Documentation
- Before implementing new functionality, review the living plan in `docs/linux-support-plan.md`.
- Update the plan whenever scope, milestones, or validation status change.
- Keep the README aligned with the implemented feature set; document any new Linux workflows or limitations.

## Implementation Priorities
1. Preserve compatibility with the existing Windows WinForms workflow while growing cross-platform support through the CLI.
2. Reuse the shared configuration artifacts (`settings.conf`, presets, shell scripts) so Windows and Linux users remain in sync.
3. Prefer managed implementations (SSH.NET, .NET APIs) over shelling out to Windows-only tooling.

## Validation Requirements
- Run and report the following commands after code changes that affect .NET projects:
  - `dotnet build OpenIPCConfigurator.Cli/OpenIPCConfigurator.Cli.csproj -c Release`
  - `dotnet test OpenIPCConfigurator.Cli.Tests/OpenIPCConfigurator.Cli.Tests.csproj`
  - `./publish-linux-cli.sh` when publishing artifacts is relevant.
- Capture additional commands in the plan’s validation log as they are executed.

## Coding Conventions
- Use idiomatic .NET 8 patterns (nullable reference types enabled, async APIs where appropriate).
- Keep new C# source files inside the `OpenIPCConfigurator.Cli` namespace and mirror the existing project structure.
- Favor descriptive logging and avoid leaking secrets; never log passwords by default.

## Coordination Checklist
- Ensure new work maintains parity with the legacy `Extern.bat` verbs listed in the plan.
- Note any deferred tasks in the "Future enhancements" section of the plan so they can be prioritized later.
- When touching shared assets (presets, shell scripts), verify they remain executable on both Windows (CRLF) and Linux (LF) as appropriate.

