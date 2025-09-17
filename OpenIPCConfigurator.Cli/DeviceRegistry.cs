using System.Collections.Generic;
using System.Linq;

namespace OpenIPCConfigurator.Cli;

internal static class DeviceRegistry
{
    private static readonly DeviceProfile OpenIpcProfile = new(
        key: "openipc",
        description: "OpenIPC FPV camera (wfb.conf format)",
        transfers: new List<FileTransfer>
        {
            new("/etc/majestic.yaml", "majestic.yaml"),
            new("/etc/wfb.conf", "wfb.conf"),
            new("/etc/telemetry.conf", "telemetry.conf")
        },
        postUploadCommands: new List<string>
        {
            "dos2unix /etc/wfb.conf /etc/telemetry.conf /etc/majestic.yaml"
        });

    private static readonly DeviceProfile OpenIpcYamlProfile = new(
        key: "openipc",
        description: "OpenIPC FPV camera (wfb.yaml format)",
        transfers: new List<FileTransfer>
        {
            new("/etc/majestic.yaml", "majestic.yaml"),
            new("/etc/wfb.yaml", "wfb.yaml")
        },
        postUploadCommands: new List<string>
        {
            "dos2unix /etc/wfb.yaml /etc/majestic.yaml"
        });

    private static readonly DeviceProfile NvrProfile = new(
        key: "nvr",
        description: "Ground station / NVR receiver",
        transfers: new List<FileTransfer>
        {
            new("/etc/vdec.conf", "vdec.conf"),
            new("/etc/wfb.conf", "wfb.conf"),
            new("/etc/telemetry.conf", "telemetry.conf")
        },
        postUploadCommands: new List<string>
        {
            "dos2unix /etc/wfb.conf /etc/telemetry.conf /etc/vdec.conf"
        });

    private static readonly DeviceProfile RadxaProfile = new(
        key: "radxa",
        description: "Radxa Zero 3W controller",
        transfers: new List<FileTransfer>
        {
            new("/etc/wifibroadcast.cfg", "wifibroadcast.cfg"),
            new("/etc/modprobe.d/wfb.conf", "wfb.conf"),
            new("/config/scripts/screen-mode", "screen-mode"),
            new("/etc/default/wifibroadcast", "wifibroadcast")
        },
        postUploadCommands: new List<string>
        {
            "dos2unix /etc/wifibroadcast.cfg /etc/modprobe.d/wfb.conf /config/scripts/screen-mode /etc/default/wifibroadcast"
        });

    public static DeviceProfile GetProfile(string deviceKey, bool useYaml)
    {
        return NormalizeKey(deviceKey) switch
        {
            "openipc" => useYaml ? OpenIpcYamlProfile : OpenIpcProfile,
            "nvr" => NvrProfile,
            "radxa" => RadxaProfile,
            _ => throw new ArgumentException($"Unknown device '{deviceKey}'. Supported values: {string.Join(", ", GetSupportedDeviceKeys())}.")
        };
    }

    public static IEnumerable<(string Key, string Description)> GetSupportedDevices()
    {
        yield return (OpenIpcProfile.Key, OpenIpcProfile.Description);
        yield return (NvrProfile.Key, NvrProfile.Description);
        yield return (RadxaProfile.Key, RadxaProfile.Description);
    }

    private static IEnumerable<string> GetSupportedDeviceKeys() => GetSupportedDevices().Select(device => device.Key);

    private static string NormalizeKey(string deviceKey)
    {
        return deviceKey.ToLowerInvariant() switch
        {
            "openipc" or "camera" or "cam" => "openipc",
            "nvr" or "vrx" or "receiver" => "nvr",
            "radxa" or "radxazero3w" or "radxa-zero-3w" => "radxa",
            _ => deviceKey.ToLowerInvariant()
        };
    }
}
