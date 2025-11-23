using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace OpenIPCConfigurator.Cli;

internal static class CommandLineParser
{
    private const string DefaultSettingsFileName = "settings.conf";

    public static bool TryParse(string[] args, out CommandLineParseResult? result, out string? errorMessage)
    {
        var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < args.Length; i++)
        {
            var token = args[i];
            if (token == "--")
            {
                // Ignore everything after "--" for now.
                continue;
            }

            if (!token.StartsWith('-'))
            {
                errorMessage = $"Unexpected argument '{token}'.";
                result = null;
                return false;
            }

            string key;
            if (token.StartsWith("--"))
            {
                key = token[2..];
            }
            else
            {
                key = token.Length switch
                {
                    2 => token[1] switch
                    {
                        'i' => "ip",
                        'p' => "password",
                        'd' => "device",
                        'u' => "username",
                        'P' => "port",
                        'w' => "workdir",
                        'o' => "output",
                        'I' => "input",
                        'r' => "reboot",
                        _ => null!
                    },
                    _ => null!
                } ?? throw new ArgumentException($"Unknown option '{token}'.");
            }

            string? value = null;
            var expectsValue = !IsFlag(key);
            if (expectsValue && i + 1 < args.Length && !args[i + 1].StartsWith('-'))
            {
                value = args[++i];
            }
            else if (expectsValue)
            {
                errorMessage = $"Option '{token}' requires a value.";
                result = null;
                return false;
            }
            else if (value is null)
            {
                value = "true";
            }

            values[key] = value;
        }

        var arguments = new CommandArguments(values);

        var deviceKey = arguments.TryGetValue("device", out var deviceRaw) && !string.IsNullOrWhiteSpace(deviceRaw)
            ? deviceRaw!
            : "openipc";

        var useYaml = arguments.GetBoolean("yaml") ?? false;
        DeviceProfile profile;
        try
        {
            profile = DeviceRegistry.GetProfile(deviceKey, useYaml);
            deviceKey = profile.Key; // Normalise the key.
        }
        catch (ArgumentException ex)
        {
            errorMessage = ex.Message;
            result = null;
            return false;
        }

        var workDirectory = arguments.TryGetValue("workdir", out var workRaw) && !string.IsNullOrWhiteSpace(workRaw)
            ? Path.GetFullPath(workRaw!)
            : Directory.GetCurrentDirectory();

        var settingsPath = arguments.TryGetValue("settings", out var settingsRaw) && !string.IsNullOrWhiteSpace(settingsRaw)
            ? settingsRaw!
            : Path.Combine(workDirectory, DefaultSettingsFileName);
        if (!Path.IsPathRooted(settingsPath))
        {
            settingsPath = Path.GetFullPath(Path.Combine(workDirectory, settingsPath));
        }

        SettingsStore? settings = null;
        if (File.Exists(settingsPath) || !arguments.ContainsKey("ip"))
        {
            settings = SettingsStore.Load(settingsPath);
        }

        var ipAddress = arguments.TryGetValue("ip", out var ipRaw) && !string.IsNullOrWhiteSpace(ipRaw)
            ? ipRaw!
            : settings?.TryGetAddress(deviceKey);
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            errorMessage = "An IP address must be provided via --ip or stored in settings.conf.";
            result = null;
            return false;
        }

        var username = arguments.TryGetValue("username", out var userRaw) && !string.IsNullOrWhiteSpace(userRaw)
            ? userRaw!
            : "root";

        if (!arguments.TryGetValue("password", out var password) || string.IsNullOrWhiteSpace(password))
        {
            errorMessage = "A password must be provided with --password.";
            result = null;
            return false;
        }

        var port = 22;
        if (arguments.TryGetValue("port", out var portRaw))
        {
            if (!int.TryParse(portRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out port))
            {
                errorMessage = $"Invalid SSH port '{portRaw}'.";
                result = null;
                return false;
            }
        }

        var rememberSettings = arguments.GetBoolean("remember") ?? true;
        if (arguments.GetBoolean("no-remember") == true)
        {
            rememberSettings = false;
        }

        var triggerReboot = arguments.GetBoolean("reboot") ?? false;

        var downloadDirectory = arguments.TryGetValue("output", out var outputRaw) && !string.IsNullOrWhiteSpace(outputRaw)
            ? Path.GetFullPath(Path.Combine(workDirectory, outputRaw!))
            : workDirectory;

        var uploadDirectory = arguments.TryGetValue("input", out var inputRaw) && !string.IsNullOrWhiteSpace(inputRaw)
            ? Path.GetFullPath(Path.Combine(workDirectory, inputRaw!))
            : workDirectory;

        var options = new CommandOptions(
            deviceKey.ToLowerInvariant(),
            ipAddress!,
            username,
            password!,
            port,
            workDirectory,
            downloadDirectory,
            uploadDirectory,
            settingsPath,
            rememberSettings,
            useYaml,
            triggerReboot);
        result = new CommandLineParseResult(options, arguments);
        errorMessage = null;
        return true;
    }

    private static bool IsFlag(string key) => key switch
    {
        "yaml" => true,
        "remember" => true,
        "no-remember" => true,
        "reboot" => true,
        _ => false
    };
}
