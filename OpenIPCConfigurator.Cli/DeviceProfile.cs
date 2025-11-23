namespace OpenIPCConfigurator.Cli;

/// <summary>
/// Describes the files and post-processing actions associated with a supported device role.
/// </summary>
internal sealed class DeviceProfile
{
    public DeviceProfile(
        string key,
        string description,
        IReadOnlyList<FileTransfer> transfers,
        IReadOnlyList<string> postUploadCommands)
    {
        Key = key;
        Description = description;
        Transfers = transfers;
        PostUploadCommands = postUploadCommands;
    }

    /// <summary>
    /// Canonical key used in settings.conf (e.g. "openipc").
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Human readable description used in help text.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Files to download/upload for this profile.
    /// </summary>
    public IReadOnlyList<FileTransfer> Transfers { get; }

    /// <summary>
    /// Commands executed after uploading configuration files.
    /// </summary>
    public IReadOnlyList<string> PostUploadCommands { get; }
}
