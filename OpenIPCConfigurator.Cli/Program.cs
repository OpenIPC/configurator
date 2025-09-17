using System;
using System.IO;
using System.Linq;

namespace OpenIPCConfigurator.Cli;

internal static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintUsage();
            return 1;
        }

        var command = args[0].ToLowerInvariant();
        if (command is "help" or "-h" or "--help")
        {
            PrintUsage();
            return 0;
        }

        var commandArgs = args.Skip(1).ToArray();
        if (commandArgs.Any(arg => arg is "-h" or "--help"))
        {
            PrintUsage(command);
            return 0;
        }

        if (!CommandLineParser.TryParse(commandArgs, out var options, out var error))
        {
            Console.Error.WriteLine(error);
            return 1;
        }

        try
        {
            return command switch
            {
                "download" => RunDownload(options!),
                "upload" => RunUpload(options!),
                "reboot" => RunReboot(options!),
                _ => UnknownCommand(command)
            };
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

    private static int RunDownload(CommandOptions options)
    {
        var profile = DeviceRegistry.GetProfile(options.DeviceKey, options.UseYaml);
        var destination = options.DownloadDirectory;
        Console.WriteLine($"Downloading configuration from {options.IpAddress} ({profile.Description}) to '{destination}'.");

        using var session = new SshSession(options.IpAddress, options.Port, options.Username, options.Password);
        session.DownloadFiles(profile.Transfers, destination);

        Console.WriteLine("Download complete.");

        PersistSettings(options, profile);
        return 0;
    }

    private static int RunUpload(CommandOptions options)
    {
        var profile = DeviceRegistry.GetProfile(options.DeviceKey, options.UseYaml);
        Console.WriteLine($"Uploading configuration from '{options.UploadDirectory}' to {options.IpAddress} ({profile.Description}).");

        using var session = new SshSession(options.IpAddress, options.Port, options.Username, options.Password);
        session.UploadFiles(profile.Transfers, options.UploadDirectory);
        session.ExecuteCommands(profile.PostUploadCommands);

        if (options.TriggerReboot)
        {
            Console.WriteLine("Issuing reboot command.");
            session.ExecuteCommands(new[] { "reboot" });
        }

        Console.WriteLine("Upload complete.");

        PersistSettings(options, profile);
        return 0;
    }

    private static int RunReboot(CommandOptions options)
    {
        var profile = DeviceRegistry.GetProfile(options.DeviceKey, options.UseYaml);
        Console.WriteLine($"Sending reboot command to {options.IpAddress} ({profile.Description}).");

        using var session = new SshSession(options.IpAddress, options.Port, options.Username, options.Password);
        session.ExecuteCommands(new[] { "reboot" });

        PersistSettings(options, profile);
        return 0;
    }

    private static void PersistSettings(CommandOptions options, DeviceProfile profile)
    {
        if (!options.RememberSettings)
        {
            return;
        }

        var store = SettingsStore.Load(options.SettingsPath);
        store.SetAddress(profile.Key, options.IpAddress);
        store.Save();
        Console.WriteLine($"Updated '{options.SettingsPath}' with the last-used {profile.Key} address.");
    }

    private static void PrintUsage(string? command = null)
    {
        Console.WriteLine("OpenIPC Configurator CLI");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  configurator-cli <command> [options]");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  download   Download configuration files from a device.");
        Console.WriteLine("  upload     Upload configuration files to a device.");
        Console.WriteLine("  reboot     Issue a reboot over SSH.");
        Console.WriteLine("  help       Show this help message.");
        Console.WriteLine();
        Console.WriteLine("Common options:");
        Console.WriteLine("  --ip, -i <address>          Device IP address (falls back to settings.conf entry).\n" +
                          "  --password, -p <password>   SSH password (required).\n" +
                          "  --device, -d <name>         Device type (default: openipc).\n" +
                          "  --username, -u <name>       SSH username (default: root).\n" +
                          "  --port, -P <number>         SSH port (default: 22).\n" +
                          "  --workdir, -w <path>        Working directory (default: current).\n" +
                          "  --input, -I <path>          Override upload source directory.\n" +
                          "  --output, -o <path>         Override download destination directory.\n" +
                          "  --settings <path>           Path to settings.conf (default: <workdir>/settings.conf).\n" +
                          "  --yaml                      Use the wfb.yaml profile for OpenIPC cameras.\n" +
                          "  --reboot, -r                Reboot the device after uploading.\n" +
                          "  --no-remember               Do not update settings.conf after completion.");
        Console.WriteLine();
        Console.WriteLine("Supported devices:");
        foreach (var device in DeviceRegistry.GetSupportedDevices())
        {
            Console.WriteLine($"  {device.Key,-8} {device.Description}");
        }
        Console.WriteLine();

        if (!string.IsNullOrEmpty(command))
        {
            Console.WriteLine($"Command '{command}' accepts the options listed above.");
            Console.WriteLine();
        }
    }

    private static int UnknownCommand(string command)
    {
        Console.Error.WriteLine($"Unknown command '{command}'. Use 'help' to list available commands.");
        return 1;
    }
}
