using System;
using System.Collections.Generic;

namespace OpenIPCConfigurator.Cli;

internal sealed class CommandArguments
{
    private static readonly HashSet<string> TrueValues = new(StringComparer.OrdinalIgnoreCase)
    {
        "true",
        "1",
        "yes",
        "y",
        "on"
    };

    private static readonly HashSet<string> FalseValues = new(StringComparer.OrdinalIgnoreCase)
    {
        "false",
        "0",
        "no",
        "n",
        "off"
    };

    private readonly IReadOnlyDictionary<string, string?> _values;

    public CommandArguments(IDictionary<string, string?> values)
    {
        _values = new Dictionary<string, string?>(values, StringComparer.OrdinalIgnoreCase);
    }

    public bool TryGetValue(string key, out string? value) => _values.TryGetValue(key, out value);

    public bool ContainsKey(string key) => _values.ContainsKey(key);

    public bool? GetBoolean(string key)
    {
        if (!_values.TryGetValue(key, out var raw) || raw is null)
        {
            return null;
        }

        if (TrueValues.Contains(raw))
        {
            return true;
        }

        if (FalseValues.Contains(raw))
        {
            return false;
        }

        return null;
    }
}
