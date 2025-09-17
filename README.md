# OpenIPC Configurator
OpenIPC Configurator for setting up FPV and URLLC devices

Please download and install the latest(very important) PuTTy before running the Configurator

---

Run the "OpenIPC Configurator.exe" file.

Enter the IP of the camera/NVR/Radxa Zero 3w.

Click Connect to receive and load the setting files.

Edit the settings.

Click Save and Reboot.

Troubleshoot:
Try to uninstall and reinstall latest version of Putty.
Check the IP and network connection.

![alt text](https://github.com/OpenIPC/configurator/blob/master/configurator.png)

---

[Manual setup](README-manual.md)

---

## Linux support

The repository now ships both a cross-platform CLI (`OpenIPCConfigurator.Cli`) and GUI (`OpenIPCConfigurator.Avalonia`) 
that mirror the complete configurator workflows without relying on Windows-only PuTTY tooling.

## Cross-platform GUI Application

The GUI version provides the same tabbed interface and functionality as the Windows Forms version but runs natively on Linux.

### Building the GUI

From the repository root, you can build a self-contained GUI application:

```bash
./publish-gui.sh
```

The script creates a standalone executable at `publish/gui/OpenIPCConfigurator.Avalonia` that includes the .NET runtime 
and doesn't require any additional dependencies on the target system.

For development, you can run the GUI directly:

```bash
dotnet run --project OpenIPCConfigurator.Avalonia
```

### GUI Features

- **Cross-platform**: Runs natively on Linux, Windows, and macOS
- **Modern UI**: Built with Avalonia UI for consistent appearance across platforms  
- **Device management**: Support for OpenIPC cameras, NVR receivers, and Radxa controllers
- **Configuration tabs**: Organized interface for basic settings, video, and WiFi configuration
- **Real-time status**: Connection status and operation progress feedback
- **Settings persistence**: Remembers last-used IP addresses and device types

## Command Line Interface

### Prerequisites

- .NET 8 SDK or runtime (`dotnet` executable) installed on your Linux host
- Network access to the OpenIPC device (default credentials remain `root` / device password)
- For GUI: X11/Wayland display server (standard on most Linux desktop environments)

### Building the CLI

From the repository root you can publish a Linux build with the helper script:

```bash
./publish-linux-cli.sh
```

The script wraps `dotnet publish` with the `linux-x64` runtime identifier and writes the output to `publish/linux/`.
If you prefer to run the command manually, invoke:

```bash
dotnet publish OpenIPCConfigurator.Cli/OpenIPCConfigurator.Cli.csproj \
  -c Release \
  -r linux-x64 \
  --self-contained false \
  -o publish/linux
```

### Usage examples

Download camera configuration:

```bash
dotnet run --project OpenIPCConfigurator.Cli -- download -i 192.168.0.10 -p op
```

Upload the edited files back to the camera and reboot afterwards:

```bash
dotnet run --project OpenIPCConfigurator.Cli -- upload -i 192.168.0.10 -p op --reboot
```

Key flags:

- `--device` / `-d` – choose between `openipc`, `nvr`, or `radxa`.
- `--yaml` – switch the OpenIPC profile to the YAML-based wireless stack (`wfb.yaml`).
- `--no-remember` – skip updating `settings.conf` with the last-used IP address.

### Additional commands

The CLI mirrors the legacy `Extern.bat` workflow, exposing commands for maintenance tasks beyond configuration sync. Highlights:

- `keys download|upload|generate` – manage drone key material for the camera or ground station.
- `uart enable|disable` – toggle console login over UART0.
- `msp` – install MSP extras, adjust OSD profiles, or configure ground-station telemetry presets.
- `services restart` – restart `wifibroadcast` or `majestic` without a full reboot.
- `sensors`, `kernel`, `scripts` – transfer binary blobs and helper scripts with automatic backup support.
- `firmware offline-upgrade` – push a `.tgz` firmware package and trigger `sysupgrade` (with optional `--force`).
- `recording`, `audio`, `mavlink`, `bittest`, `video` – tweak runtime features such as DVR recording, audio, telemetry level, or video sizing.
- `radxa reset`, `camera factory-reset`, `air-manager install`, `pixelpilot install`, `alink deploy` – perform higher-level platform maintenance, including Radxa resets and PixelPilot provisioning.

Run `dotnet run --project OpenIPCConfigurator.Cli -- help <command>` for the full option set on any subcommand.

For full option details run `dotnet run --project OpenIPCConfigurator.Cli -- help`.
