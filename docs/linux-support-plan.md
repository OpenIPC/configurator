# Linux Support Analysis and Plan

## Codebase Overview
- **Project type:** The repository currently ships a Visual Basic .NET 8 Windows Forms application (`OpenIPC Configurator.vbproj`). The UI logic in `Configurator.vb` drives user interactions and orchestrates device communication.
- **External tooling:** Instead of using native .NET networking APIs, the UI shells out to `extern.bat`, a very large Windows batch file that wraps PuTTY's `pscp`/`plink` utilities to copy configuration files, reboot devices, and run maintenance commands.
- **Configuration state:** A local `settings.conf` file stores the last-used IP addresses for different device roles (`openipc`, `nvr`, `radxa`). Numerous preset YAML/CFG files live alongside the executable and are assumed to be in the working directory.
- **Platform coupling:**
  - The WinForms project targets `net8.0-windows` and uses Windows-only UI components.
  - Automation is delegated to a `.bat` script and PuTTY executables, tying the workflow to Windows.
  - Several helper assets (resolution scripts, presets) are shell-friendly and can already run on Linux, but there is no orchestrator for them.

## Linux Support Goals
1. Allow users on Linux to download, edit, and upload the same configuration bundles without relying on PuTTY or Windows batch files.
2. Preserve the existing workflow expectations (file names, settings persistence) so Windows and Linux users share the same artifacts.
3. Create a foundation that can later be reused by the WinForms client to remove the PuTTY dependency altogether.

## Proposed Approach
### High-level strategy
- Introduce a cross-platform .NET 8 CLI (`OpenIPCConfigurator.Cli`) that encapsulates SSH/SCP interactions using the managed [`SSH.NET`](https://github.com/sshnet/SSH.NET) library (surface namespace `Renci.SshNet`).
- Model each supported device role (OpenIPC camera, NVR receiver, Radxa controller) as a profile that declares which remote files to transfer and which post-upload commands to execute (`dos2unix`, `reboot`, etc.).
- Reuse the existing `settings.conf` format so the CLI can remember the last-used IP addresses per role and remain compatible with the Windows app.
- Document how Linux users can invoke the CLI for download/upload/reboot operations.

### Command parity snapshot
The legacy `Extern.bat` script orchestrates PuTTY utilities for a wide range of actions. To keep scope manageable, phase 1 concentrates on the essential configuration lifecycle, while cataloguing the remaining tasks for follow-up work.

| Script verb            | Purpose                                              | Phase 1 CLI parity             | Notes |
|------------------------|------------------------------------------------------|-------------------------------|-------|
| `dl`, `dlyaml`         | Download OpenIPC camera configs                      | ✅ `download` command          | YAML support controlled via `--yaml` switch. |
| `ul`, `ulr`, `ulyamlr` | Upload OpenIPC configs (optional reboot)             | ✅ `upload` + `--reboot` flag  | CLI emits reboot only when requested. |
| `dlvrx`, `ulvrx(r)`    | Ground-station config sync                           | ✅ `--device nvr` profile      | Same transfers and `dos2unix` calls. |
| `dlwfbng`, `ulwfbng(r)`| Radxa controller config sync                         | ✅ `--device radxa` profile    | Upload ensures required files exist. |
| `rb`                   | Remote reboot                                        | ✅ dedicated `reboot` command  | Aligns with GUI reboot button flow. |
| Remaining verbs (keys, UART, firmware, extras, etc.) | Maintenance & advanced tweaks | ⏳ Future iterations | Documented under "Future enhancements" backlog. |

### Implementation plan (phase 1 – executed now)
1. **Create documentation** (this file) to capture the current architecture and Linux-support strategy.
2. **Add the CLI project** targeting `net8.0`, reference `SSH.NET`, and wire it into the existing solution for discoverability.
3. **Implement core services** inside the CLI:
   - `DeviceProfile` definitions encapsulating remote/local file mappings and post-upload commands.
   - `SettingsStore` to load/update the shared `settings.conf` file.
   - `SshSession` helper to perform secure copy and remote command execution via SSH.NET.
4. **Expose user commands** (`download`, `upload`, `reboot`) with argument parsing (`--ip`, `--password`, `--device`, optional paths) so Linux users can orchestrate the workflows from a terminal.
5. **Update project documentation** (`README.md`) with a Linux usage section describing prerequisites (e.g., .NET SDK) and CLI examples.
6. **Validate** by building the solution on Linux and, where possible, unit-testing the parsing logic.

### Future enhancements (phase 2 – deferred)
- Refactor the Windows Forms application to consume the new SSH helper instead of `extern.bat`, enabling a single cross-platform implementation.
- Extend the CLI to cover advanced maintenance commands (keys management, presets, telemetry toggles) that are currently mirrored in `extern.bat`.
- Package the CLI as a standalone binary or container image for easier distribution on Linux distributions.

## Validation Strategy
- Automated: `dotnet build` for all projects, and targeted unit tests around configuration parsing.
- Manual: Spot-check generated/updated files (`settings.conf`, downloaded configs) and run a dry-run connection against a test host (future work when hardware is available).

## Current validation status
- ✅ Ubuntu 24.04 container with .NET SDK 8.0.119:
  - `dotnet build OpenIPCConfigurator.Cli/OpenIPCConfigurator.Cli.csproj -c Release`
  - `dotnet test OpenIPCConfigurator.Cli.Tests/OpenIPCConfigurator.Cli.Tests.csproj`
  - `dotnet publish OpenIPCConfigurator.Cli/OpenIPCConfigurator.Cli.csproj -c Release -r linux-x64 --self-contained false -o publish/linux`

## Risks and Mitigations
- **SSH credential handling:** Using `SSH.NET` avoids shelling out but still requires storing passwords in memory. Ensure parameters are only logged when verbosity is requested.
- **Incomplete command coverage:** The CLI initially covers the primary download/upload/reboot flows; documentation will clearly state the supported subset and point to future work for advanced features.
- **Environment prerequisites:** Document the need for the .NET 8 SDK/runtime so Linux users can build or run the CLI.

