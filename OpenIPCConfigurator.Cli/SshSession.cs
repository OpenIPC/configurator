using Renci.SshNet;

namespace OpenIPCConfigurator.Cli;

internal sealed class SshSession : IDisposable
{
    private readonly SshClient _sshClient;
    private readonly ScpClient _scpClient;
    private bool _isConnected;

    public SshSession(string host, int port, string username, string password)
    {
        var connectionInfo = new PasswordConnectionInfo(host, port, username, password);
        _sshClient = new SshClient(connectionInfo);
        _scpClient = new ScpClient(connectionInfo);
    }

    public void Connect()
    {
        if (_isConnected)
        {
            return;
        }

        _sshClient.Connect();
        _scpClient.Connect();
        _isConnected = true;
    }

    public void DownloadFiles(IEnumerable<FileTransfer> transfers, string destinationDirectory)
    {
        Connect();
        Directory.CreateDirectory(destinationDirectory);

        foreach (var transfer in transfers)
        {
            var localPath = Path.Combine(destinationDirectory, transfer.LocalName);
            using var localStream = File.Create(localPath);
            _scpClient.Download(transfer.RemotePath, localStream);
        }
    }

    public void UploadFiles(IEnumerable<FileTransfer> transfers, string sourceDirectory)
    {
        Connect();

        foreach (var transfer in transfers)
        {
            var localPath = Path.Combine(sourceDirectory, transfer.LocalName);
            if (!File.Exists(localPath))
            {
                throw new FileNotFoundException($"Required file '{transfer.LocalName}' not found in '{sourceDirectory}'.", localPath);
            }

            using var localStream = File.OpenRead(localPath);
            _scpClient.Upload(localStream, transfer.RemotePath);
        }
    }

    public void ExecuteCommands(IEnumerable<string> commands)
    {
        Connect();

        foreach (var command in commands)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                continue;
            }

            using var cmd = _sshClient.CreateCommand(command);
            _ = cmd.Execute();
            if (cmd.ExitStatus != 0)
            {
                throw new InvalidOperationException($"Command '{command}' failed with exit code {cmd.ExitStatus}: {cmd.Error}");
            }
        }
    }

    public void Dispose()
    {
        if (_scpClient.IsConnected)
        {
            _scpClient.Disconnect();
        }

        if (_sshClient.IsConnected)
        {
            _sshClient.Disconnect();
        }

        _scpClient.Dispose();
        _sshClient.Dispose();
    }
}
