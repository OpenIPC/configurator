using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using OpenIPCConfigurator.Cli;

namespace OpenIPCConfigurator.Shared;

public class OpenIPCService : IOpenIPCService
{
    public event EventHandler<string>? StatusChanged;
    public event EventHandler<string>? OperationCompleted;
    public event EventHandler<string>? ErrorOccurred;

    private void OnStatusChanged(string status) => StatusChanged?.Invoke(this, status);
    private void OnOperationCompleted(string operation) => OperationCompleted?.Invoke(this, operation);
    private void OnErrorOccurred(string error) => ErrorOccurred?.Invoke(this, error);

    public async Task<bool> TestConnectionAsync(string ipAddress, string password, DeviceType deviceType)
    {
        try
        {
            OnStatusChanged($"Testing connection to {ipAddress}...");
            
            var deviceKey = deviceType.ToKey();
            using var session = new SshSession(ipAddress, 22, "root", password);
            
            // Try a simple command to test connectivity
            var result = await Task.Run(() =>
            {
                try
                {
                    session.ExecuteCommands(new[] { "echo 'connection test'" });
                    return true;
                }
                catch (Exception ex)
                {
                    OnErrorOccurred($"SSH connection failed: {ex.Message}");
                    return false;
                }
            });

            if (result)
            {
                OnStatusChanged($"Connected to {ipAddress}");
                OnOperationCompleted("Connection test successful");
                
                // Also test what configuration files are available
                try
                {
                    OnStatusChanged("Checking device configuration format...");
                    using var checkSession = new SshSession(ipAddress, 22, "root", password);
                    checkSession.ExecuteCommands(new[] { "ls -la /etc/ | grep -E '(wfb\\.conf|wfb\\.yaml|majestic\\.yaml)' || echo 'No standard config files found'" });
                }
                catch
                {
                    // Ignore this check if it fails
                }
            }
            else
            {
                OnErrorOccurred($"Failed to connect to {ipAddress} - check IP address and password");
            }

            return result;
        }
        catch (Exception ex)
        {
            OnErrorOccurred($"Connection test failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DownloadConfigurationAsync(string ipAddress, string password, DeviceType deviceType, bool useYaml = false, string? outputDir = null)
    {
        try
        {
            OnStatusChanged($"Downloading configuration from {ipAddress}...");
            
            var args = BuildArgs("download", ipAddress, password, deviceType, useYaml, outputDir: outputDir);
            var result = await RunCliCommandAsync(args);
            
            if (result)
            {
                OnOperationCompleted("Configuration download completed");
            }
            else
            {
                OnErrorOccurred("Configuration download failed");
            }

            return result;
        }
        catch (Exception ex)
        {
            OnErrorOccurred($"Download failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UploadConfigurationAsync(string ipAddress, string password, DeviceType deviceType, bool useYaml = false, bool reboot = false, string? inputDir = null)
    {
        try
        {
            OnStatusChanged($"Uploading configuration to {ipAddress}...");
            
            var args = BuildArgs("upload", ipAddress, password, deviceType, useYaml, reboot, inputDir);
            var result = await RunCliCommandAsync(args);
            
            if (result)
            {
                OnOperationCompleted("Configuration upload completed");
            }
            else
            {
                OnErrorOccurred("Configuration upload failed");
            }

            return result;
        }
        catch (Exception ex)
        {
            OnErrorOccurred($"Upload failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RebootDeviceAsync(string ipAddress, string password, DeviceType deviceType)
    {
        try
        {
            OnStatusChanged($"Rebooting device {ipAddress}...");
            
            var args = BuildArgs("reboot", ipAddress, password, deviceType);
            var result = await RunCliCommandAsync(args);
            
            if (result)
            {
                OnOperationCompleted("Device reboot initiated");
            }
            else
            {
                OnErrorOccurred("Device reboot failed");
            }

            return result;
        }
        catch (Exception ex)
        {
            OnErrorOccurred($"Reboot failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> GenerateKeysAsync(string ipAddress, string password)
    {
        try
        {
            OnStatusChanged("Generating encryption keys...");
            var args = BuildArgs("keys", ipAddress, password, DeviceType.OpenIPC, subcommand: "generate");
            var result = await RunCliCommandAsync(args);
            
            if (result)
            {
                OnOperationCompleted("Encryption keys generated");
            }
            else
            {
                OnErrorOccurred("Key generation failed");
            }

            return result;
        }
        catch (Exception ex)
        {
            OnErrorOccurred($"Key generation failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DownloadKeysAsync(string ipAddress, string password, string target = "camera")
    {
        try
        {
            OnStatusChanged($"Downloading {target} keys...");
            var args = BuildArgs("keys", ipAddress, password, DeviceType.OpenIPC, subcommand: "download", extraArgs: new[] { "--target", target });
            var result = await RunCliCommandAsync(args);
            
            if (result)
            {
                OnOperationCompleted($"{target} keys downloaded");
            }
            else
            {
                OnErrorOccurred($"{target} key download failed");
            }

            return result;
        }
        catch (Exception ex)
        {
            OnErrorOccurred($"Key download failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UploadKeysAsync(string ipAddress, string password, string target = "camera")
    {
        try
        {
            OnStatusChanged($"Uploading {target} keys...");
            var args = BuildArgs("keys", ipAddress, password, DeviceType.OpenIPC, subcommand: "upload", extraArgs: new[] { "--target", target });
            var result = await RunCliCommandAsync(args);
            
            if (result)
            {
                OnOperationCompleted($"{target} keys uploaded");
            }
            else
            {
                OnErrorOccurred($"{target} key upload failed");
            }

            return result;
        }
        catch (Exception ex)
        {
            OnErrorOccurred($"Key upload failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> EnableUartAsync(string ipAddress, string password)
    {
        try
        {
            OnStatusChanged("Enabling UART console...");
            var args = BuildArgs("uart", ipAddress, password, DeviceType.OpenIPC, subcommand: "enable");
            var result = await RunCliCommandAsync(args);
            
            if (result)
            {
                OnOperationCompleted("UART console enabled");
            }
            else
            {
                OnErrorOccurred("UART enable failed");
            }

            return result;
        }
        catch (Exception ex)
        {
            OnErrorOccurred($"UART enable failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DisableUartAsync(string ipAddress, string password)
    {
        try
        {
            OnStatusChanged("Disabling UART console...");
            var args = BuildArgs("uart", ipAddress, password, DeviceType.OpenIPC, subcommand: "disable");
            var result = await RunCliCommandAsync(args);
            
            if (result)
            {
                OnOperationCompleted("UART console disabled");
            }
            else
            {
                OnErrorOccurred("UART disable failed");
            }

            return result;
        }
        catch (Exception ex)
        {
            OnErrorOccurred($"UART disable failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> InstallMspAirAsync(string ipAddress, string password)
    {
        try
        {
            OnStatusChanged("Installing MSP air components...");
            var args = BuildArgs("msp", ipAddress, password, DeviceType.OpenIPC, subcommand: "install-air");
            var result = await RunCliCommandAsync(args);
            
            if (result)
            {
                OnOperationCompleted("MSP air components installed");
            }
            else
            {
                OnErrorOccurred("MSP air installation failed");
            }

            return result;
        }
        catch (Exception ex)
        {
            OnErrorOccurred($"MSP air installation failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> InstallMspGroundAsync(string ipAddress, string password)
    {
        try
        {
            OnStatusChanged("Installing MSP ground components...");
            var args = BuildArgs("msp", ipAddress, password, DeviceType.NVR, subcommand: "install-ground");
            var result = await RunCliCommandAsync(args);
            
            if (result)
            {
                OnOperationCompleted("MSP ground components installed");
            }
            else
            {
                OnErrorOccurred("MSP ground installation failed");
            }

            return result;
        }
        catch (Exception ex)
        {
            OnErrorOccurred($"MSP ground installation failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RemoveMspExtraAsync(string ipAddress, string password)
    {
        try
        {
            OnStatusChanged("Removing MSP extra components...");
            var args = BuildArgs("msp", ipAddress, password, DeviceType.OpenIPC, subcommand: "remove-extra");
            var result = await RunCliCommandAsync(args);
            
            if (result)
            {
                OnOperationCompleted("MSP extra components removed");
            }
            else
            {
                OnErrorOccurred("MSP extra removal failed");
            }

            return result;
        }
        catch (Exception ex)
        {
            OnErrorOccurred($"MSP extra removal failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SetOsdModeAsync(string ipAddress, string password, string mode)
    {
        try
        {
            OnStatusChanged($"Setting OSD mode to {mode}...");
            var args = BuildArgs("msp", ipAddress, password, DeviceType.Radxa, subcommand: "set-osd", extraArgs: new[] { "--mode", mode });
            var result = await RunCliCommandAsync(args);
            
            if (result)
            {
                OnOperationCompleted($"OSD mode set to {mode}");
            }
            else
            {
                OnErrorOccurred($"OSD mode setting failed");
            }

            return result;
        }
        catch (Exception ex)
        {
            OnErrorOccurred($"OSD mode setting failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ConfigureMspGroundStationAsync(string ipAddress, string password)
    {
        try
        {
            OnStatusChanged("Configuring MSP ground station...");
            var args = BuildArgs("msp", ipAddress, password, DeviceType.NVR, subcommand: "configure-groundstation");
            var result = await RunCliCommandAsync(args);
            
            if (result)
            {
                OnOperationCompleted("MSP ground station configured");
            }
            else
            {
                OnErrorOccurred("MSP ground station configuration failed");
            }

            return result;
        }
        catch (Exception ex)
        {
            OnErrorOccurred($"MSP ground station configuration failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RestartServiceAsync(string ipAddress, string password, string serviceName)
    {
        try
        {
            OnStatusChanged($"Restarting {serviceName} service...");
            var args = BuildArgs("services", ipAddress, password, DeviceType.OpenIPC, subcommand: "restart", extraArgs: new[] { "--service", serviceName });
            var result = await RunCliCommandAsync(args);
            
            if (result)
            {
                OnOperationCompleted($"{serviceName} service restarted");
            }
            else
            {
                OnErrorOccurred($"{serviceName} service restart failed");
            }

            return result;
        }
        catch (Exception ex)
        {
            OnErrorOccurred($"Service restart failed: {ex.Message}");
            return false;
        }
    }

    // Add implementations for all other methods following the same pattern...
    // For brevity, I'll add placeholders for the remaining methods

    public Task<bool> UploadSensorAsync(string ipAddress, string password, string fileName) =>
        ExecuteCommandAsync("sensors", "upload", ipAddress, password, DeviceType.OpenIPC, new[] { "--file", fileName }, $"Uploading sensor {fileName}");

    public Task<bool> DownloadSensorAsync(string ipAddress, string password, string fileName) =>
        ExecuteCommandAsync("sensors", "download", ipAddress, password, DeviceType.OpenIPC, new[] { "--file", fileName }, $"Downloading sensor {fileName}");

    public Task<bool> UploadKernelModuleAsync(string ipAddress, string password, string fileName) =>
        ExecuteCommandAsync("kernel", "upload", ipAddress, password, DeviceType.OpenIPC, new[] { "--file", fileName }, $"Uploading kernel module {fileName}");

    public Task<bool> DownloadKernelModuleAsync(string ipAddress, string password, string fileName) =>
        ExecuteCommandAsync("kernel", "download", ipAddress, password, DeviceType.OpenIPC, new[] { "--file", fileName }, $"Downloading kernel module {fileName}");

    public Task<bool> UploadScriptsAsync(string ipAddress, string password) =>
        ExecuteCommandAsync("scripts", "upload", ipAddress, password, DeviceType.OpenIPC, null, "Uploading scripts");

    public Task<bool> DownloadScriptsAsync(string ipAddress, string password) =>
        ExecuteCommandAsync("scripts", "download", ipAddress, password, DeviceType.OpenIPC, null, "Downloading scripts");

    public Task<bool> OfflineFirmwareUpgradeAsync(string ipAddress, string password, string archivePath, string profile, bool force = false) =>
        ExecuteCommandAsync("firmware", "offline-upgrade", ipAddress, password, DeviceType.OpenIPC, 
            force ? new[] { "--archive", archivePath, "--profile", profile, "--force" } : new[] { "--archive", archivePath, "--profile", profile },
            "Performing offline firmware upgrade");

    public Task<bool> EnableRecordingAsync(string ipAddress, string password) =>
        ExecuteCommandAsync("recording", "enable", ipAddress, password, DeviceType.OpenIPC, null, "Enabling recording");

    public Task<bool> DisableRecordingAsync(string ipAddress, string password) =>
        ExecuteCommandAsync("recording", "disable", ipAddress, password, DeviceType.OpenIPC, null, "Disabling recording");

    public Task<bool> EnableAudioAsync(string ipAddress, string password) =>
        ExecuteCommandAsync("audio", "enable", ipAddress, password, DeviceType.OpenIPC, null, "Enabling audio");

    public Task<bool> DisableAudioAsync(string ipAddress, string password) =>
        ExecuteCommandAsync("audio", "disable", ipAddress, password, DeviceType.OpenIPC, null, "Disabling audio");

    public Task<bool> SetMavlinkLevelAsync(string ipAddress, string password, int level) =>
        ExecuteCommandAsync("mavlink", "set-level", ipAddress, password, DeviceType.NVR, new[] { "--level", level.ToString() }, $"Setting MAVLink level to {level}");

    public Task<bool> ResetRadxaAsync(string ipAddress, string password) =>
        ExecuteCommandAsync("radxa", "reset", ipAddress, password, DeviceType.Radxa, null, "Resetting Radxa controller");

    public Task<bool> FactoryResetCameraAsync(string ipAddress, string password) =>
        ExecuteCommandAsync("camera", "factory-reset", ipAddress, password, DeviceType.OpenIPC, null, "Factory resetting camera");

    public Task<bool> InstallAirManagerAsync(string ipAddress, string password) =>
        ExecuteCommandAsync("air-manager", "install", ipAddress, password, DeviceType.NVR, null, "Installing Air Manager");

    public Task<bool> InstallPixelPilotAsync(string ipAddress, string password) =>
        ExecuteCommandAsync("pixelpilot", "install", ipAddress, password, DeviceType.NVR, null, "Installing PixelPilot");

    public Task<bool> DeployAlinkAsync(string ipAddress, string password, string profile) =>
        ExecuteCommandAsync("alink", "deploy", ipAddress, password, DeviceType.OpenIPC, new[] { "--profile", profile }, $"Deploying alink with profile {profile}");

    public Task<bool> SetCropModeAsync(string ipAddress, string password, int mode) =>
        ExecuteCommandAsync("crop", "set", ipAddress, password, DeviceType.OpenIPC, new[] { "--mode", mode.ToString() }, $"Setting crop mode to {mode}");

    public Task<bool> DisableCropAsync(string ipAddress, string password) =>
        ExecuteCommandAsync("crop", "disable", ipAddress, password, DeviceType.OpenIPC, null, "Disabling crop mode");

    public Task<bool> RunBitTestAsync(string ipAddress, string password, string mlinkValue) =>
        ExecuteCommandAsync("bittest", "run", ipAddress, password, DeviceType.OpenIPC, new[] { "--mlink", mlinkValue }, $"Running bitrate test with mlink {mlinkValue}");

    public Task<bool> FixVideoResolutionAsync(string ipAddress, string password) =>
        ExecuteCommandAsync("video", "fix-resolution", ipAddress, password, DeviceType.OpenIPC, null, "Fixing video resolution");

    private async Task<bool> ExecuteCommandAsync(string command, string subcommand, string ipAddress, string password, DeviceType deviceType, string[]? extraArgs, string statusMessage)
    {
        try
        {
            OnStatusChanged(statusMessage);
            var args = BuildArgs(command, ipAddress, password, deviceType, subcommand: subcommand, extraArgs: extraArgs);
            var result = await RunCliCommandAsync(args);
            
            if (result)
            {
                OnOperationCompleted(statusMessage + " completed");
            }
            else
            {
                OnErrorOccurred(statusMessage + " failed");
            }

            return result;
        }
        catch (Exception ex)
        {
            OnErrorOccurred($"{statusMessage} failed: {ex.Message}");
            return false;
        }
    }

    private static string[] BuildArgs(string command, string ipAddress, string password, DeviceType deviceType, bool useYaml = false, bool reboot = false, string? inputDir = null, string? outputDir = null, string? subcommand = null, string[]? extraArgs = null)
    {
        var args = new List<string>();
        
        args.Add(command);
        if (!string.IsNullOrEmpty(subcommand))
        {
            args.Add(subcommand);
        }

        args.AddRange(new[] { "--ip", ipAddress, "--password", password, "--device", deviceType.ToKey() });

        if (useYaml && deviceType == DeviceType.OpenIPC)
        {
            args.Add("--yaml");
        }

        if (reboot)
        {
            args.Add("--reboot");
        }

        if (!string.IsNullOrEmpty(inputDir))
        {
            args.AddRange(new[] { "--input", inputDir });
        }

        if (!string.IsNullOrEmpty(outputDir))
        {
            args.AddRange(new[] { "--output", outputDir });
        }

        if (extraArgs != null)
        {
            args.AddRange(extraArgs);
        }

        return args.ToArray();
    }

    private async Task<bool> RunCliCommandAsync(string[] args)
    {
        try
        {
            return await Task.Run(() => OpenIPCConfigurator.Cli.Program.Main(args) == 0);
        }
        catch (Exception)
        {
            return false;
        }
    }
}
