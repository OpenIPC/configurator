using System;

namespace OpenIPCConfigurator.Shared;

public enum DeviceType
{
    OpenIPC,
    NVR,
    Radxa
}

public static class DeviceTypeExtensions
{
    public static string ToKey(this DeviceType deviceType)
    {
        return deviceType switch
        {
            DeviceType.OpenIPC => "openipc",
            DeviceType.NVR => "nvr", 
            DeviceType.Radxa => "radxa",
            _ => throw new ArgumentOutOfRangeException(nameof(deviceType))
        };
    }
    
    public static DeviceType FromKey(string key)
    {
        return key.ToLowerInvariant() switch
        {
            "openipc" or "camera" or "cam" => DeviceType.OpenIPC,
            "nvr" or "vrx" or "receiver" => DeviceType.NVR,
            "radxa" or "radxazero3w" or "radxa-zero-3w" => DeviceType.Radxa,
            _ => throw new ArgumentException($"Unknown device key: {key}")
        };
    }
}
