using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenIPCConfigurator.Cli;

public sealed class SettingsStore
{
    private static readonly string[] DefaultKeys = { "openipc", "nvr", "radxa" };

    private readonly Dictionary<string, string> _values;
    private readonly string _path;

    private SettingsStore(string path, Dictionary<string, string> values)
    {
        _path = path;
        _values = values;
    }

    public static SettingsStore Load(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("The settings path cannot be empty.", nameof(path));
        }

        var resolvedPath = Path.GetFullPath(path);
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (File.Exists(resolvedPath))
        {
            foreach (var line in File.ReadLines(resolvedPath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#", StringComparison.Ordinal))
                {
                    continue;
                }

                var parts = line.Split(':', 2);
                if (parts.Length == 2)
                {
                    values[parts[0].Trim()] = parts[1].Trim();
                }
            }
        }
        else
        {
            foreach (var key in DefaultKeys)
            {
                values[key] = "192.168.0.1";
            }
        }

        return new SettingsStore(resolvedPath, values);
    }

    public string? TryGetAddress(string key)
    {
        if (!_values.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }

    public void SetAddress(string key, string ipAddress)
    {
        _values[key] = ipAddress;
    }

    public void Save()
    {
        var directory = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var lines = new List<string>();
        foreach (var key in DefaultKeys)
        {
            if (_values.TryGetValue(key, out var value))
            {
                lines.Add($"{key}:{value}");
            }
            else
            {
                lines.Add($"{key}:");
            }
        }

        foreach (var extra in _values)
        {
            if (DefaultKeys.Contains(extra.Key, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            lines.Add($"{extra.Key}:{extra.Value}");
        }

        File.WriteAllLines(_path, lines);
    }
}
