
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace OpenIPCConfigurator.Cli;

internal static class Program
{
    private sealed record CommandHandler(string Name, string Description, Func<string[], int> Execute, string Usage);

    private static readonly CommandHandler[] CommandHandlers =
    {
        new("download", "Download configuration files from a device.", RunDownload, "Downloads configuration files to --output (defaults to the working directory)."),
        new("upload", "Upload configuration files to a device.", RunUpload, "Uploads configuration files from --input (defaults to the working directory)."),
        new("reboot", "Issue a remote reboot over SSH.", RunReboot, "Triggers an immediate reboot on the target device."),
        new("keys", "Manage FPV encryption keys.", RunKeys, "Subcommands: download --target <camera|gs>, upload --target <camera|gs>, generate."),
        new("uart", "Toggle UART0 console access.", RunUart, "Subcommands: enable, disable."),
        new("msp", "Manage MSP/OSD helpers.", RunMsp, "Subcommands: install-air, install-ground, remove-extra, set-osd --mode <air|ground>, configure-groundstation."),
        new("services", "Control background services.", RunServices, "Usage: services restart --service <wifibroadcast|majestic>."),
        new("sensors", "Synchronise sensor binaries.", RunSensors, "Subcommands: upload --file <name>, download --file <name>."),
        new("kernel", "Synchronise kernel module payloads.", RunKernel, "Subcommands: upload --file <name>, download --file <name>."),
        new("scripts", "Synchronise helper shell scripts.", RunScripts, "Subcommands: upload, download."),
        new("firmware", "Perform offline firmware upgrades.", RunFirmware, "Subcommands: offline-upgrade --archive <tgz> --profile <name> [--force]."),
        new("recording", "Toggle onboard DVR recording.", RunRecording, "Subcommands: enable, disable."),
        new("audio", "Toggle audio capture.", RunAudio, "Subcommands: enable, disable."),
        new("mavlink", "Configure MAVLink telemetry level.", RunMavlink, "Subcommands: set-level --level <1|2>."),
        new("radxa", "Radxa controller helpers.", RunRadxa, "Subcommands: reset."),
        new("camera", "Camera maintenance helpers.", RunCamera, "Subcommands: factory-reset."),
        new("air-manager", "Install the OpenIPC air manager package.", RunAirManager, "Subcommands: install."),
        new("pixelpilot", "Install the PixelPilot OSD suite.", RunPixelpilot, "Subcommands: install."),
        new("alink", "Deploy alink drone binaries and profiles.", RunAlink, "Subcommands: deploy --profile <name>."),
        new("crop", "Configure image crop presets.", RunCrop, "Subcommands: set --mode <1-5>, disable."),
        new("bittest", "Run bitrate stability presets.", RunBittest, "Subcommands: run --mlink <value>."),
        new("video", "Video pipeline utilities.", RunVideo, "Subcommands: fix-resolution."),
    };

    private static readonly Dictionary<string, CommandHandler> CommandLookup =
        CommandHandlers.ToDictionary(handler => handler.Name, StringComparer.OrdinalIgnoreCase);

    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintUsage();
            return 1;
        }

        var commandName = args[0];
        if (IsHelpCommand(commandName))
        {
            if (args.Length > 1 && CommandLookup.TryGetValue(args[1], out var handler))
            {
                PrintCommandHelp(handler);
            }
            else
            {
                PrintUsage();
            }

            return 0;
        }

        if (!CommandLookup.TryGetValue(commandName, out var command))
        {
            Console.Error.WriteLine($"Unknown command '{commandName}'. Use 'help' to list available commands.");
            return 1;
        }

        var commandArgs = args.Skip(1).ToArray();
        if (commandArgs.Any(arg => arg is "-h" or "--help"))
        {
            PrintCommandHelp(command);
            return 0;
        }

        try
        {
            return command.Execute(commandArgs);
        }
        catch (FileNotFoundException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            if (ex.InnerException is not null)
            {
                Console.Error.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }

            return 1;
        }
    }

    private static bool IsHelpCommand(string command) => command is "help" or "-h" or "--help";

    private static int RunDownload(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        var profile = DeviceRegistry.GetProfile(options.DeviceKey, options.UseYaml);
        Console.WriteLine($"Downloading configuration from {options.IpAddress} ({profile.Description}) to '{options.DownloadDirectory}'.");

        using var session = CreateSession(options);
        session.DownloadFiles(profile.Transfers, options.DownloadDirectory);

        Console.WriteLine("Download complete.");
        PersistSettings(options);
        return 0;
    }

    private static int RunUpload(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        var profile = DeviceRegistry.GetProfile(options.DeviceKey, options.UseYaml);
        Console.WriteLine($"Uploading configuration from '{options.UploadDirectory}' to {options.IpAddress} ({profile.Description}).");

        using var session = CreateSession(options);
        session.UploadFiles(profile.Transfers, options.UploadDirectory);
        session.ExecuteCommands(profile.PostUploadCommands);

        if (options.TriggerReboot)
        {
            Console.WriteLine("Issuing reboot command.");
            session.ExecuteCommands(new[] { "reboot" });
        }

        Console.WriteLine("Upload complete.");
        PersistSettings(options);
        return 0;
    }

    private static int RunReboot(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        var profile = DeviceRegistry.GetProfile(options.DeviceKey, options.UseYaml);
        Console.WriteLine($"Sending reboot command to {options.IpAddress} ({profile.Description}).");

        using var session = CreateSession(options);
        session.ExecuteCommands(new[] { "reboot" });

        PersistSettings(options);
        return 0;
    }

    private static int RunKeys(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("The keys command requires a subcommand (download, upload, generate).");
            return 1;
        }

        var subcommand = args[0].ToLowerInvariant();
        var commandArgs = args.Skip(1).ToArray();

        return subcommand switch
        {
            "download" => RunKeysDownload(commandArgs),
            "upload" => RunKeysUpload(commandArgs),
            "generate" => RunKeysGenerate(commandArgs),
            _ => UnknownSubcommand("keys", subcommand)
        };
    }

    private static int RunKeysDownload(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        var target = GetArgument(parseResult.Arguments, "target")?.ToLowerInvariant() ?? "camera";

        (string RemotePath, string Description) mapping = target switch
        {
            "camera" or "cam" => ("/etc/drone.key", "camera"),
            "gs" or "ground" or "groundstation" => ("/root/drone.key", "ground station"),
            _ => default
        };

        if (mapping == default)
        {
            Console.Error.WriteLine("Unknown key target. Use --target camera or --target gs.");
            return 1;
        }

        Console.WriteLine($"Downloading {mapping.Description} key from {options.IpAddress} to '{options.DownloadDirectory}'.");

        using var session = CreateSession(options);
        session.DownloadFiles(new[] { new FileTransfer(mapping.RemotePath, "drone.key") }, options.DownloadDirectory);

        Console.WriteLine("Download complete.");
        PersistSettings(options);
        return 0;
    }

    private static int RunKeysUpload(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        var target = GetArgument(parseResult.Arguments, "target")?.ToLowerInvariant() ?? "camera";

        (string RemotePath, string Description, bool CopyGsKey) mapping = target switch
        {
            "camera" or "cam" => ("/etc/drone.key", "camera", false),
            "gs" or "ground" or "groundstation" => ("/etc/drone.key", "ground station", true),
            _ => default
        };

        if (mapping == default)
        {
            Console.Error.WriteLine("Unknown key target. Use --target camera or --target gs.");
            return 1;
        }

        var localKey = ResolveInputFile(options, "drone.key");
        Console.WriteLine($"Uploading {mapping.Description} key from '{localKey}' to {options.IpAddress}.");

        using var session = CreateSession(options);
        session.UploadFile(localKey, mapping.RemotePath);

        if (mapping.CopyGsKey)
        {
            session.ExecuteCommands(new[] { "cp /etc/drone.key /etc/gs.key" });
        }

        PersistSettings(options);
        return 0;
    }

    private static int RunKeysGenerate(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        Console.WriteLine($"Generating fresh drone keys on {options.IpAddress}.");

        using var session = CreateSession(options);
        session.ExecuteCommands(new[] { "wfb_keygen && cp /root/gs.key /etc/" });

        PersistSettings(options);
        return 0;
    }

    private static int RunUart(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("The uart command requires a subcommand (enable or disable).");
            return 1;
        }

        var subcommand = args[0].ToLowerInvariant();
        var commandArgs = args.Skip(1).ToArray();

        return subcommand switch
        {
            "enable" => RunUartEnable(commandArgs),
            "disable" => RunUartDisable(commandArgs),
            _ => UnknownSubcommand("uart", subcommand)
        };
    }

    private static int RunUartEnable(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        Console.WriteLine($"Disabling console login on UART0 for {options.IpAddress}.");

        using var session = CreateSession(options);
        session.ExecuteCommands(new[]
        {
            "sed -i 's/console::respawn\\/sbin\\/getty -L console 0 vt100/#console::respawn\\/sbin\\/getty -L console 0 vt100/' /etc/inittab && reboot"
        });

        PersistSettings(options);
        return 0;
    }

    private static int RunUartDisable(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        Console.WriteLine($"Enabling console login on UART0 for {options.IpAddress}.");

        using var session = CreateSession(options);
        session.ExecuteCommands(new[]
        {
            "sed -i 's/#console::respawn\\/sbin\\/getty -L console 0 vt100/console::respawn\\/sbin\\/getty -L console 0 vt100/' /etc/inittab && reboot"
        });

        PersistSettings(options);
        return 0;
    }

    private static int RunMsp(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("The msp command requires a subcommand.");
            return 1;
        }

        var subcommand = args[0].ToLowerInvariant();
        var commandArgs = args.Skip(1).ToArray();

        return subcommand switch
        {
            "install-air" => RunMspInstallAir(commandArgs),
            "install-ground" => RunMspInstallGround(commandArgs),
            "remove-extra" => RunMspRemoveExtra(commandArgs),
            "set-osd" => RunMspSetOsd(commandArgs),
            "configure-groundstation" => RunMspConfigureGroundStation(commandArgs),
            _ => UnknownSubcommand("msp", subcommand)
        };
    }

    private static int RunMspInstallAir(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        if (!EnsureDevice(options, "openipc", "msp install-air"))
        {
            return 1;
        }

        using var session = CreateSession(options);
        var wifibroadcastPath = ResolveWorkPath(options, Path.Combine("reset", "wifibroadcast"));
        var menuPath = ResolveWorkPath(options, "vtxmenu.ini");

        session.UploadFile(wifibroadcastPath, "/usr/bin/wifibroadcast");
        session.UploadFile(menuPath, "/etc/vtxmenu.ini");
        session.ExecuteCommands(new[] { "dos2unix /usr/bin/wifibroadcast /etc/vtxmenu.ini && wifibroadcast reset && reboot" });

        PersistSettings(options);
        return 0;
    }

    private static int RunMspInstallGround(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        using var session = CreateSession(options);
        var wifibroadcastPath = ResolveWorkPath(options, Path.Combine("reset", "wifibroadcast_gs"));
        var menuPath = ResolveWorkPath(options, "vtxmenu.ini");

        session.ExecuteCommands(new[] { "killall wifibroadcast || true" });
        session.UploadFile(wifibroadcastPath, "/usr/bin/wifibroadcast_gs");
        session.UploadFile(menuPath, "/etc/vtxmenu.ini");
        session.ExecuteCommands(new[]
        {
            "dos2unix /usr/bin/wifibroadcast_gs /etc/vtxmenu.ini && mv /usr/bin/wifibroadcast_gs /usr/bin/wifibroadcast && chmod +x /usr/bin/wifibroadcast && wifibroadcast reset && reboot"
        });

        PersistSettings(options);
        return 0;
    }

    private static int RunMspRemoveExtra(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        using var session = CreateSession(options);
        session.ExecuteCommands(new[] { "sed -i 's/sleep 5/#sleep 5/' /usr/bin/wifibroadcast && reboot" });
        PersistSettings(options);
        return 0;
    }
    private static int RunMspSetOsd(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        var mode = GetArgument(parseResult.Arguments, "mode")?.ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(mode))
        {
            Console.Error.WriteLine("The set-osd subcommand requires --mode <air|ground>.");
            return 1;
        }

        string command = mode switch
        {
            "air" => "sed -i 's/render = ground/render = air/' /config/scripts/osd && reboot",
            "ground" or "gs" => "sed -i 's/render = air/render = ground/' /config/scripts/osd && reboot",
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(command))
        {
            Console.Error.WriteLine("Unknown OSD mode. Use --mode air or --mode ground.");
            return 1;
        }

        using var session = CreateSession(options);
        session.ExecuteCommands(new[] { command });
        PersistSettings(options);
        return 0;
    }

    private static int RunMspConfigureGroundStation(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        using var session = CreateSession(options);
        const string command1 = "sed -i '/pixelpilot --osd --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4/c\\pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4 \"&\"' /config/scripts/stream.sh";
        const string command2 = "sed -i '/pixelpilot --osd --screen-mode $SCREEN_MODE/c\\pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE \"&\"' /config/scripts/stream.sh && reboot";
        session.ExecuteCommands(new[] { command1, command2 });
        PersistSettings(options);
        return 0;
    }

    private static int RunServices(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("The services command requires a subcommand (currently only restart).");
            return 1;
        }

        var subcommand = args[0].ToLowerInvariant();
        if (subcommand != "restart")
        {
            return UnknownSubcommand("services", subcommand);
        }

        var commandArgs = args.Skip(1).ToArray();
        if (!TryParseOptions(commandArgs, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        var service = GetArgument(parseResult.Arguments, "service")?.ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(service))
        {
            Console.Error.WriteLine("The restart subcommand requires --service <wifibroadcast|majestic>.");
            return 1;
        }

        string remoteCommand = service switch
        {
            "wifibroadcast" => "wifibroadcast start",
            "majestic" => "killall -1 majestic",
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(remoteCommand))
        {
            Console.Error.WriteLine("Unknown service. Supported values: wifibroadcast, majestic.");
            return 1;
        }

        using var session = CreateSession(options);
        session.ExecuteCommands(new[] { remoteCommand });
        PersistSettings(options);
        return 0;
    }

    private static int RunSensors(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("The sensors command requires a subcommand (upload or download).");
            return 1;
        }

        var subcommand = args[0].ToLowerInvariant();
        var commandArgs = args.Skip(1).ToArray();

        return subcommand switch
        {
            "upload" => RunSensorsUpload(commandArgs),
            "download" => RunSensorsDownload(commandArgs),
            _ => UnknownSubcommand("sensors", subcommand)
        };
    }

    private static int RunSensorsUpload(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        var fileName = GetArgument(parseResult.Arguments, "file");
        if (string.IsNullOrWhiteSpace(fileName))
        {
            Console.Error.WriteLine("The sensors upload command requires --file <name>.");
            return 1;
        }

        string localPath;
        try
        {
            localPath = ResolveInputFile(options, fileName, "sensors");
        }
        catch (FileNotFoundException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }

        var remotePath = $"/etc/sensors/{Path.GetFileName(localPath)}";
        Console.WriteLine($"Uploading sensor profile '{Path.GetFileName(localPath)}' to {options.IpAddress}.");

        using var session = CreateSession(options);
        session.UploadFile(localPath, remotePath);
        session.ExecuteCommands(new[] { $"yaml-cli -s .isp.sensorConfig {remotePath} && reboot" });

        PersistSettings(options);
        return 0;
    }

    private static int RunSensorsDownload(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        var fileName = GetArgument(parseResult.Arguments, "file");
        if (string.IsNullOrWhiteSpace(fileName))
        {
            Console.Error.WriteLine("The sensors download command requires --file <name>.");
            return 1;
        }

        var backupDir = Path.Combine(options.WorkDirectory, "backup");
        Console.WriteLine($"Downloading sensor profile '{fileName}' from {options.IpAddress} to '{backupDir}'.");

        using var session = CreateSession(options);
        session.DownloadFiles(new[] { new FileTransfer($"/etc/sensors/{fileName}", fileName) }, backupDir);

        PersistSettings(options);
        return 0;
    }

    private static int RunKernel(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("The kernel command requires a subcommand (upload or download).");
            return 1;
        }

        var subcommand = args[0].ToLowerInvariant();
        var commandArgs = args.Skip(1).ToArray();

        return subcommand switch
        {
            "upload" => RunKernelUpload(commandArgs),
            "download" => RunKernelDownload(commandArgs),
            _ => UnknownSubcommand("kernel", subcommand)
        };
    }

    private static int RunKernelUpload(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        var fileName = GetArgument(parseResult.Arguments, "file");
        if (string.IsNullOrWhiteSpace(fileName))
        {
            Console.Error.WriteLine("The kernel upload command requires --file <name>.");
            return 1;
        }

        string localPath;
        try
        {
            localPath = ResolveInputFile(options, fileName);
        }
        catch (FileNotFoundException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }

        var remotePath = $"/lib/modules/4.9.84/sigmastar/{Path.GetFileName(localPath)}";
        Console.WriteLine($"Uploading kernel module '{Path.GetFileName(localPath)}' to {options.IpAddress}.");

        using var session = CreateSession(options);
        session.UploadFile(localPath, remotePath);
        PersistSettings(options);
        return 0;
    }

    private static int RunKernelDownload(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        var fileName = GetArgument(parseResult.Arguments, "file");
        if (string.IsNullOrWhiteSpace(fileName))
        {
            Console.Error.WriteLine("The kernel download command requires --file <name>.");
            return 1;
        }

        var backupDir = Path.Combine(options.WorkDirectory, "backup");
        Console.WriteLine($"Downloading kernel module '{fileName}' from {options.IpAddress} to '{backupDir}'.");

        using var session = CreateSession(options);
        session.DownloadFiles(new[] { new FileTransfer($"/lib/modules/4.9.84/sigmastar/{fileName}", fileName) }, backupDir);

        PersistSettings(options);
        return 0;
    }

    private static int RunScripts(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("The scripts command requires a subcommand (upload or download).");
            return 1;
        }

        var subcommand = args[0].ToLowerInvariant();
        var commandArgs = args.Skip(1).ToArray();

        return subcommand switch
        {
            "upload" => RunScriptsUpload(commandArgs),
            "download" => RunScriptsDownload(commandArgs),
            _ => UnknownSubcommand("scripts", subcommand)
        };
    }

    private static int RunScriptsUpload(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        var scriptFiles = Directory.Exists(options.UploadDirectory)
            ? Directory.EnumerateFiles(options.UploadDirectory, "*.sh", SearchOption.TopDirectoryOnly).ToList()
            : new List<string>();

        if (scriptFiles.Count == 0)
        {
            Console.Error.WriteLine($"No .sh files found in '{options.UploadDirectory}'.");
            return 1;
        }

        using var session = CreateSession(options);
        foreach (var script in scriptFiles)
        {
            var name = Path.GetFileName(script);
            session.UploadFile(script, $"/root/{name}");
        }

        var channelsPath = Path.Combine(options.UploadDirectory, "channels.sh");
        if (File.Exists(channelsPath))
        {
            session.UploadFile(channelsPath, "/usr/bin/channels.sh");
        }

        session.ExecuteCommands(new[]
        {
            "rm -f /root/channels.sh",
            "chmod +x /root/*.sh",
            "chmod +x /usr/bin/channels.sh"
        });

        PersistSettings(options);
        return 0;
    }

    private static int RunScriptsDownload(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        var transfers = new List<FileTransfer>
        {
            new("/usr/bin/channels.sh", "channels.sh"),
            new("/root/816.sh", "816.sh"),
            new("/root/1080.sh", "1080.sh"),
            new("/root/1080b.sh", "1080b.sh"),
            new("/root/1264.sh", "1264.sh"),
            new("/root/3K.sh", "3K.sh"),
            new("/root/4K.sh", "4K.sh"),
            new("/root/1184p100.sh", "1184p100.sh"),
            new("/root/1304p80.sh", "1304p80.sh"),
            new("/root/1440p60.sh", "1440p60.sh"),
            new("/root/1920p30.sh", "1920p30.sh"),
            new("/root/1080p60.sh", "1080p60.sh"),
            new("/root/720p120.sh", "720p120.sh"),
            new("/root/720p100.sh", "720p100.sh"),
            new("/root/720p90.sh", "720p90.sh"),
            new("/root/720p60.sh", "720p60.sh"),
            new("/root/1080p120.sh", "1080p120.sh"),
            new("/root/1248p90.sh", "1248p90.sh"),
            new("/root/1416p70.sh", "1416p70.sh"),
            new("/root/kill.sh", "kill.sh")
        };

        Console.WriteLine($"Downloading helper scripts to '{options.DownloadDirectory}'.");
        using var session = CreateSession(options);
        session.DownloadFiles(transfers, options.DownloadDirectory);

        PersistSettings(options);
        return 0;
    }

    private static int RunFirmware(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("The firmware command requires a subcommand (offline-upgrade).");
            return 1;
        }

        var subcommand = args[0].ToLowerInvariant();
        var commandArgs = args.Skip(1).ToArray();

        return subcommand switch
        {
            "offline-upgrade" => RunFirmwareOfflineUpgrade(commandArgs),
            _ => UnknownSubcommand("firmware", subcommand)
        };
    }

    private static int RunFirmwareOfflineUpgrade(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        var archiveOption = GetArgument(parseResult.Arguments, "archive") ?? GetArgument(parseResult.Arguments, "package");
        var profile = GetArgument(parseResult.Arguments, "profile");
        var force = parseResult.Arguments.GetBoolean("force") ?? false;

        if (string.IsNullOrWhiteSpace(archiveOption) || string.IsNullOrWhiteSpace(profile))
        {
            Console.Error.WriteLine("offline-upgrade requires --archive <tgz> and --profile <name>.");
            return 1;
        }

        var archivePath = archiveOption.EndsWith(".tgz", StringComparison.OrdinalIgnoreCase)
            ? archiveOption
            : archiveOption + ".tgz";

        string localArchive;
        try
        {
            localArchive = ResolveInputFile(options, archivePath);
        }
        catch (FileNotFoundException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }

        var fileName = Path.GetFileName(localArchive);
        var remoteArchive = $"/tmp/{fileName}";
        var remoteTar = $"/tmp/{Path.GetFileNameWithoutExtension(fileName)}.tar";

        Console.WriteLine($"Uploading firmware package '{fileName}' to {options.IpAddress}.");
        using var session = CreateSession(options);
        session.UploadFile(localArchive, remoteArchive);

        var upgradeCommand = $"gzip -d {remoteArchive} && tar -xvf {remoteTar} -C /tmp && sysupgrade --kernel=/tmp/uImage.{profile} --rootfs=/tmp/rootfs.squashfs.{profile} -n" + (force ? " -f" : string.Empty);
        session.ExecuteCommands(new[] { upgradeCommand });

        PersistSettings(options);
        return 0;
    }

    private static int RunRecording(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("The recording command requires a subcommand (enable or disable).");
            return 1;
        }

        var subcommand = args[0].ToLowerInvariant();
        var commandArgs = args.Skip(1).ToArray();

        return subcommand switch
        {
            "enable" => RunRecordingEnable(commandArgs),
            "disable" => RunRecordingDisable(commandArgs),
            _ => UnknownSubcommand("recording", subcommand)
        };
    }

    private static int RunRecordingEnable(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        using var session = CreateSession(options);
        session.ExecuteCommands(new[] { "yaml-cli -s .records.enabled true && reboot" });
        PersistSettings(options);
        return 0;
    }

    private static int RunRecordingDisable(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        using var session = CreateSession(options);
        session.ExecuteCommands(new[] { "yaml-cli -s .records.enabled false && reboot" });
        PersistSettings(options);
        return 0;
    }

    private static int RunAudio(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("The audio command requires a subcommand (enable or disable).");
            return 1;
        }

        var subcommand = args[0].ToLowerInvariant();
        var commandArgs = args.Skip(1).ToArray();

        return subcommand switch
        {
            "enable" => RunAudioEnable(commandArgs),
            "disable" => RunAudioDisable(commandArgs),
            _ => UnknownSubcommand("audio", subcommand)
        };
    }

    private static int RunAudioEnable(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        using var session = CreateSession(options);
        session.ExecuteCommands(new[] { "yaml-cli -s .audio.enabled true && reboot" });
        PersistSettings(options);
        return 0;
    }

    private static int RunAudioDisable(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        using var session = CreateSession(options);
        session.ExecuteCommands(new[] { "yaml-cli -s .audio.enabled false && reboot" });
        PersistSettings(options);
        return 0;
    }

    private static int RunMavlink(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("The mavlink command requires a subcommand (set-level).");
            return 1;
        }

        var subcommand = args[0].ToLowerInvariant();
        var commandArgs = args.Skip(1).ToArray();

        return subcommand switch
        {
            "set-level" => RunMavlinkSetLevel(commandArgs),
            _ => UnknownSubcommand("mavlink", subcommand)
        };
    }

    private static int RunMavlinkSetLevel(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        var levelValue = GetArgument(parseResult.Arguments, "level");
        if (string.IsNullOrWhiteSpace(levelValue) || !int.TryParse(levelValue, out var level) || (level != 1 && level != 2))
        {
            Console.Error.WriteLine("set-level requires --level 1 or --level 2.");
            return 1;
        }

        string command1 = level switch
        {
            2 => "sed -i '/pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4 \"&\"/c\\pixelpilot --osd --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4 --osd-telem-lvl 2 \"&\"' /config/scripts/stream.sh",
            _ => "sed -i '/pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4 \"&\"/c\\pixelpilot --osd --screen-mode $SCREEN_MODE --dvr-framerate $REC_FPS --dvr-fmp4 --dvr record_${current_date}.mp4 --osd-telem-lvl 1 \"&\"' /config/scripts/stream.sh"
        };

        string command2 = level switch
        {
            2 => "sed -i '/pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE \"&\"/c\\pixelpilot --osd --screen-mode $SCREEN_MODE --osd-telem-lvl 2 \"&\"' /config/scripts/stream.sh && reboot",
            _ => "sed -i '/pixelpilot --osd --osd-elements video,wfbng --screen-mode $SCREEN_MODE \"&\"/c\\pixelpilot --osd --screen-mode $SCREEN_MODE --osd-telem-lvl 1 \"&\"' /config/scripts/stream.sh && reboot"
        };

        using var session = CreateSession(options);
        session.ExecuteCommands(new[] { command1, command2 });
        PersistSettings(options);
        return 0;
    }
    private static int RunRadxa(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("The radxa command requires a subcommand (reset).");
            return 1;
        }

        var subcommand = args[0].ToLowerInvariant();
        var commandArgs = args.Skip(1).ToArray();

        return subcommand switch
        {
            "reset" => RunRadxaReset(commandArgs),
            _ => UnknownSubcommand("radxa", subcommand)
        };
    }

    private static int RunRadxaReset(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        if (!EnsureDevice(options, "radxa", "radxa reset"))
        {
            return 1;
        }

        using var session = CreateSession(options);
        session.UploadFile(ResolveWorkPath(options, Path.Combine("reset", "wifibroadcast.cfg")), "/etc/wifibroadcast.cfg");
        session.UploadFile(ResolveWorkPath(options, Path.Combine("reset", "wfb.conf")), "/etc/modprobe.d/wfb.conf");
        session.UploadFile(ResolveWorkPath(options, Path.Combine("reset", "wifibroadcast")), "/etc/default/wifibroadcast");
        session.UploadFile(ResolveWorkPath(options, Path.Combine("reset", "stream.sh")), "/config/scripts/stream.sh");
        session.UploadFile(ResolveWorkPath(options, Path.Combine("reset", "osd")), "/config/scripts/osd");
        session.UploadFile(ResolveWorkPath(options, Path.Combine("reset", "osd.json")), "/config/scripts/osd.json");
        session.UploadFile(ResolveWorkPath(options, Path.Combine("reset", "rec-fps")), "/config/scripts/rec-fps");
        session.UploadFile(ResolveWorkPath(options, Path.Combine("reset", "screen-mode")), "/config/scripts/screen-mode");
        session.UploadFile(ResolveWorkPath(options, Path.Combine("reset", "alink_gs.conf")), "/config/alink_gs.conf");

        session.ExecuteCommands(new[]
        {
            "dos2unix /etc/wifibroadcast.cfg /etc/modprobe.d/wfb.conf /etc/default/wifibroadcast /config/scripts/screen-mode /config/scripts/osd /config/scripts/osd.json /config/scripts/stream.sh /config/scripts/rec-fps /config/alink_gs.conf && reboot"
        });

        PersistSettings(options);
        return 0;
    }

    private static int RunCamera(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("The camera command requires a subcommand (factory-reset).");
            return 1;
        }

        var subcommand = args[0].ToLowerInvariant();
        var commandArgs = args.Skip(1).ToArray();

        return subcommand switch
        {
            "factory-reset" => RunCameraFactoryReset(commandArgs),
            _ => UnknownSubcommand("camera", subcommand)
        };
    }

    private static int RunCameraFactoryReset(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        using var session = CreateSession(options);
        session.ExecuteCommands(new[] { "firstboot" });
        PersistSettings(options);
        return 0;
    }

    private static int RunAirManager(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("The air-manager command requires a subcommand (install).");
            return 1;
        }

        var subcommand = args[0].ToLowerInvariant();
        var commandArgs = args.Skip(1).ToArray();

        return subcommand switch
        {
            "install" => RunAirManagerInstall(commandArgs),
            _ => UnknownSubcommand("air-manager", subcommand)
        };
    }

    private static int RunAirManagerInstall(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        var archivePath = ResolveWorkPath(options, "OpenIPC-air_manager.tar");
        using var session = CreateSession(options);
        session.UploadFile(archivePath, "/OpenIPC-air_manager.tar");
        session.ExecuteCommands(new[] { "tar -xvf /OpenIPC-air_manager.tar && cd /root/OpenIPC-air_manager/ && chmod +x install.sh && ./install.sh 10.5.0.10" });
        PersistSettings(options);
        return 0;
    }

    private static int RunPixelpilot(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("The pixelpilot command requires a subcommand (install).");
            return 1;
        }

        var subcommand = args[0].ToLowerInvariant();
        var commandArgs = args.Skip(1).ToArray();

        return subcommand switch
        {
            "install" => RunPixelpilotInstall(commandArgs),
            _ => UnknownSubcommand("pixelpilot", subcommand)
        };
    }

    private static int RunPixelpilotInstall(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        var artifactPath = ResolveWorkPath(options, "artifact.zip");
        Console.WriteLine("Extracting PixelPilot artifacts...");
        ZipFile.ExtractToDirectory(artifactPath, options.WorkDirectory, overwriteFiles: true);

        var pixelpilotBin = ResolveWorkPath(options, "pixelpilot");
        var menuScript = ResolveWorkPath(options, "gsmenu.sh");
        var configJson = ResolveWorkPath(options, "config_osd.json");

        using var session = CreateSession(options);
        const string prepCommand = "systemctl stop openipc && awk 'NR==2 {$0=\"sed -i '\''s/\\r//'\'' /config/setup.txt \"&\"\"} {print }' /config/scripts/stream.sh > /config/scripts/stream2.sh && rm /config/scripts/stream.sh && mv /config/scripts/stream2.sh /config/scripts/stream.sh";
        session.ExecuteCommands(new[] { prepCommand });
        session.UploadFile(pixelpilotBin, "/usr/local/bin/pixelpilot");
        session.UploadFile(menuScript, "/usr/local/bin/gsmenu.sh");
        session.UploadFile(pixelpilotBin, "/usr/local/etc/pixelpilot/pixelpilot");
        session.UploadFile(configJson, "/usr/local/etc/pixelpilot/config_osd.json");
        session.ExecuteCommands(new[] { "reboot" });

        PersistSettings(options);
        return 0;
    }

    }

    private static int RunAlink(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("The alink command requires a subcommand (deploy).");
            return 1;
        }

        var subcommand = args[0].ToLowerInvariant();
        var commandArgs = args.Skip(1).ToArray();

        return subcommand switch
        {
            "deploy" => RunAlinkDeploy(commandArgs),
            _ => UnknownSubcommand("alink", subcommand)
        };
    }

    private static int RunAlinkDeploy(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        var profileName = GetArgument(parseResult.Arguments, "profile");
        if (string.IsNullOrWhiteSpace(profileName))
        {
            Console.Error.WriteLine("The alink deploy command requires --profile <name>.");
            return 1;
        }

        using var session = CreateSession(options);
        session.ExecuteCommands(new[] { "killall alink_drone || true" });
        session.UploadFile(ResolveWorkPath(options, "alink_drone"), "/usr/bin/alink_drone");
        session.UploadFile(ResolveWorkPath(options, "yaml-cli-multi"), "/usr/bin/yaml-cli-multi");
        session.UploadFile(ResolveWorkPath(options, "wlan_adapters.yaml"), "/etc/wlan_adapters.yaml");
        session.UploadFile(ResolveWorkPath(options, "alink.conf"), "/etc/alink.conf");
        session.UploadFile(ResolveWorkPath(options, Path.Combine("txprofiles", $"{profileName}.conf")), $"/etc/{profileName}.conf");
        session.ExecuteCommands(new[]
        {
            $"mv /etc/{profileName}.conf /etc/txprofiles.conf && dos2unix /etc/alink.conf /etc/wlan_adapters.yaml /etc/txprofiles.conf && chmod +x /usr/bin/alink_drone && chmod +x /usr/bin/yaml-cli-multi && reboot"
        });

        PersistSettings(options);
        return 0;
    }

    private static int RunCrop(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("The crop command requires a subcommand (set or disable).");
            return 1;
        }

        var subcommand = args[0].ToLowerInvariant();
        var commandArgs = args.Skip(1).ToArray();

        return subcommand switch
        {
            "set" => RunCropSet(commandArgs),
            "disable" => RunCropDisable(commandArgs),
            _ => UnknownSubcommand("crop", subcommand)
        };
    }

    private static int RunCropSet(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        var modeValue = GetArgument(parseResult.Arguments, "mode");
        if (string.IsNullOrWhiteSpace(modeValue) || !int.TryParse(modeValue, out var mode) || mode < 1 || mode > 5)
        {
            Console.Error.WriteLine("The crop set command requires --mode between 1 and 5.");
            return 1;
        }

        var commands = new Dictionary<int, string>
        {
            [1] = "sed -i '/echo setprecrop/c\\echo setprecrop 0 0 904 0 2436 1828 \">\" /proc/mi_modules/mi_vpe/mi_vpe0' /etc/rc.local",
            [2] = "sed -i '/echo setprecrop/c\\echo setprecrop 0 0 904 0 2560 1920 \">\" /proc/mi_modules/mi_vpe/mi_vpe0' /etc/rc.local",
            [3] = "sed -i '/echo setprecrop/c\\echo setprecrop 0 0 904 0 1440 1080 \">\" /proc/mi_modules/mi_vpe/mi_vpe0' /etc/rc.local",
            [4] = "sed -i '/echo setprecrop/c\\echo setprecrop 0 0 240 0 1440 1080 \">\" /proc/mi_modules/mi_vpe/mi_vpe0' /etc/rc.local",
            [5] = "sed -i '/echo setprecrop/c\\echo setprecrop 0 0 904 0 1440 1080 \">\" /proc/mi_modules/mi_vpe/mi_vpe0' /etc/rc.local"
        };

        using var session = CreateSession(options);
        session.UploadFile(ResolveWorkPath(options, Path.Combine("reset", "rc.local")), "/etc/rc.local");
        session.ExecuteCommands(new[] { "dos2unix /etc/rc.local" });
        session.ExecuteCommands(new[] { commands[mode] });
        PersistSettings(options);
        return 0;
    }

    private static int RunCropDisable(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        using var session = CreateSession(options);
        session.ExecuteCommands(new[] { "sed -i '/sleep 0.5/d' /etc/rc.local && sed -i '/echo setprecrop*/d' /etc/rc.local" });
        PersistSettings(options);
        return 0;
    }

    private static int RunBittest(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("The bittest command requires a subcommand (run).");
            return 1;
        }

        var subcommand = args[0].ToLowerInvariant();
        var commandArgs = args.Skip(1).ToArray();

        return subcommand switch
        {
            "run" => RunBittestRun(commandArgs),
            _ => UnknownSubcommand("bittest", subcommand)
        };
    }

    private static int RunBittestRun(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        var mlinkValue = GetArgument(parseResult.Arguments, "mlink");
        if (string.IsNullOrWhiteSpace(mlinkValue))
        {
            Console.Error.WriteLine("The bittest run command requires --mlink <value>.");
            return 1;
        }

        using var session = CreateSession(options);
        session.ExecuteCommands(new[] { $"yaml-cli -s .fpv.noiseLevel 0 && wifibroadcast cli -s .wireless.mlink {mlinkValue} && wifibroadcast start" });
        PersistSettings(options);
        return 0;
    }

    private static int RunVideo(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("The video command requires a subcommand (fix-resolution).");
            return 1;
        }

        var subcommand = args[0].ToLowerInvariant();
        var commandArgs = args.Skip(1).ToArray();

        return subcommand switch
        {
            "fix-resolution" => RunVideoFixResolution(commandArgs),
            _ => UnknownSubcommand("video", subcommand)
        };
    }

    private static int RunVideoFixResolution(string[] args)
    {
        if (!TryParseOptions(args, out var parseResult))
        {
            return 1;
        }

        var options = parseResult!.Options;
        using var session = CreateSession(options);
        session.ExecuteCommands(new[] { "yaml-cli -s .video0.size 1920x1080" });
        PersistSettings(options);
        return 0;
    }

    private static bool TryParseOptions(string[] args, out CommandLineParseResult? result)
    {
        if (!CommandLineParser.TryParse(args, out result, out var error))
        {
            Console.Error.WriteLine(error);
            return false;
        }

        return true;
    }

    private static void PersistSettings(CommandOptions options)
    {
        if (!options.RememberSettings)
        {
            return;
        }

        var profile = DeviceRegistry.GetProfile(options.DeviceKey, options.UseYaml);
        var store = SettingsStore.Load(options.SettingsPath);
        store.SetAddress(profile.Key, options.IpAddress);
        store.Save();
        Console.WriteLine($"Updated '{options.SettingsPath}' with the last-used {profile.Key} address.");
    }

    private static SshSession CreateSession(CommandOptions options) => new(options.IpAddress, options.Port, options.Username, options.Password);

    private static void PrintUsage()
    {
        Console.WriteLine("OpenIPC Configurator CLI");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  configurator-cli <command> [options]");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        foreach (var handler in CommandHandlers)
        {
            Console.WriteLine($"  {handler.Name,-15}{handler.Description}");
        }

        Console.WriteLine("  help           Show this help message.");
        Console.WriteLine();
        PrintCommonOptions();
        Console.WriteLine("Use 'configurator-cli help <command>' to view command-specific details.");
    }

    private static void PrintCommandHelp(CommandHandler handler)
    {
        Console.WriteLine($"Command: {handler.Name}");
        Console.WriteLine(handler.Description);
        Console.WriteLine();
        PrintCommonOptions();
        Console.WriteLine(handler.Usage);
    }

    private static void PrintCommonOptions()
    {
        Console.WriteLine("Common options:");
        Console.WriteLine("  --ip, -i <address>          Device IP address (falls back to settings.conf entry).");
        Console.WriteLine("  --password, -p <password>   SSH password (required).");
        Console.WriteLine("  --device, -d <name>         Device type (default: openipc).");
        Console.WriteLine("  --username, -u <name>       SSH username (default: root).");
        Console.WriteLine("  --port, -P <number>         SSH port (default: 22).");
        Console.WriteLine("  --workdir, -w <path>        Working directory (default: current).");
        Console.WriteLine("  --input, -I <path>          Override upload source directory.");
        Console.WriteLine("  --output, -o <path>         Override download destination directory.");
        Console.WriteLine("  --settings <path>           Path to settings.conf (default: <workdir>/settings.conf).");
        Console.WriteLine("  --yaml                      Use the wfb.yaml profile for OpenIPC cameras.");
        Console.WriteLine("  --reboot, -r                Reboot the device after uploading configuration files.");
        Console.WriteLine("  --no-remember               Do not update settings.conf after completion.");
        Console.WriteLine();
        Console.WriteLine("Supported devices:");
        foreach (var device in DeviceRegistry.GetSupportedDevices())
        {
            Console.WriteLine($"  {device.Key,-8} {device.Description}");
        }
        Console.WriteLine();
    }

    private static int UnknownSubcommand(string command, string subcommand)
    {
        Console.Error.WriteLine($"Unknown subcommand '{subcommand}' for '{command}'. Use 'configurator-cli help {command}' for usage details.");
        return 1;
    }

    private static string? GetArgument(CommandArguments arguments, string key)
    {
        return arguments.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : null;
    }

    private static bool EnsureDevice(CommandOptions options, string expectedKey, string commandName)
    {
        if (!string.Equals(options.DeviceKey, expectedKey, StringComparison.OrdinalIgnoreCase))
        {
            Console.Error.WriteLine($"The '{commandName}' command requires --device {expectedKey}.");
            return false;
        }

        return true;
    }

    private static string ResolveInputFile(CommandOptions options, string file, params string[] fallbackDirectories)
    {
        if (string.IsNullOrWhiteSpace(file))
        {
            throw new ArgumentException("File name cannot be empty.", nameof(file));
        }

        var searchPaths = new List<string>();
        if (Path.IsPathRooted(file))
        {
            searchPaths.Add(file);
        }
        else
        {
            searchPaths.Add(Path.Combine(options.UploadDirectory, file));
            searchPaths.Add(Path.Combine(options.WorkDirectory, file));
            foreach (var folder in fallbackDirectories)
            {
                searchPaths.Add(Path.Combine(options.WorkDirectory, folder, file));
            }
        }

        foreach (var candidate in searchPaths)
        {
            if (File.Exists(candidate))
            {
                return Path.GetFullPath(candidate);
            }
        }

        var checkedPaths = string.Join(", ", searchPaths.Select(path => Path.GetDirectoryName(path) ?? path).Distinct());
        throw new FileNotFoundException($"Required file '{file}' not found. Checked: {checkedPaths}.", searchPaths[0]);
    }

    private static string ResolveWorkPath(CommandOptions options, string relativePath)
    {
        var path = Path.Combine(options.WorkDirectory, relativePath);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Required file '{relativePath}' not found in '{options.WorkDirectory}'.", path);
        }

        return Path.GetFullPath(path);
    }
}
