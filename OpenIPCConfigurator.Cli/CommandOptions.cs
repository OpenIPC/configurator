namespace OpenIPCConfigurator.Cli;

internal sealed class CommandOptions
{
    public CommandOptions(
        string deviceKey,
        string ipAddress,
        string username,
        string password,
        int port,
        string workDirectory,
        string downloadDirectory,
        string uploadDirectory,
        string settingsPath,
        bool rememberSettings,
        bool useYaml,
        bool triggerReboot)
    {
        DeviceKey = deviceKey;
        IpAddress = ipAddress;
        Username = username;
        Password = password;
        Port = port;
        WorkDirectory = workDirectory;
        DownloadDirectory = downloadDirectory;
        UploadDirectory = uploadDirectory;
        SettingsPath = settingsPath;
        RememberSettings = rememberSettings;
        UseYaml = useYaml;
        TriggerReboot = triggerReboot;
    }

    public string DeviceKey { get; }

    public string IpAddress { get; }

    public string Username { get; }

    public string Password { get; }

    public int Port { get; }

    public string WorkDirectory { get; }

    public string DownloadDirectory { get; }

    public string UploadDirectory { get; }

    public string SettingsPath { get; }

    public bool RememberSettings { get; }

    public bool UseYaml { get; }

    public bool TriggerReboot { get; }
}
