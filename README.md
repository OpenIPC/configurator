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

## Linux command line support

The repository now ships a cross-platform CLI (`OpenIPCConfigurator.Cli`) that mirrors the common download/upload workflows
without relying on Windows-only PuTTY tooling.

### Prerequisites

- .NET 8 SDK or runtime (`dotnet` executable) installed on your Linux host.
- Network access to the OpenIPC device (default credentials remain `root` / device password).

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

For full option details run `dotnet run --project OpenIPCConfigurator.Cli -- help`.
