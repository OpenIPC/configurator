namespace OpenIPCConfigurator.Cli;

/// <summary>
/// Represents a single file that should be transferred between the host and a remote device.
/// </summary>
/// <param name="RemotePath">Absolute path of the file on the remote device.</param>
/// <param name="LocalName">File name to use on the local filesystem.</param>
internal sealed record FileTransfer(string RemotePath, string LocalName);
