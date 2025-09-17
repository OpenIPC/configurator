using System;
using System.IO;
using OpenIPCConfigurator.Cli;
using Xunit;

namespace OpenIPCConfigurator.Cli.Tests;

public class CommandLineParserTests
{
    [Fact]
    public void TryParse_ReturnsOptions_WithMinimalArguments()
    {
        var args = new[] { "--ip", "192.168.0.2", "--password", "secret" };

        var result = CommandLineParser.TryParse(args, out var options, out var error);

        Assert.True(result);
        Assert.Null(error);
        Assert.NotNull(options);
        Assert.Equal("openipc", options!.DeviceKey);
        Assert.Equal("192.168.0.2", options.IpAddress);
        Assert.Equal("secret", options.Password);
        Assert.Equal(22, options.Port);
    }

    [Fact]
    public void TryParse_ReadsIpFromSettings_WhenMissing()
    {
        using var temp = new TempDirectory();
        var settingsPath = Path.Combine(temp.Path, "settings.conf");
        File.WriteAllLines(settingsPath, new[]
        {
            "openipc:10.0.0.5",
            "nvr:",
            "radxa:"
        });

        var args = new[]
        {
            "--password", "pw",
            "--workdir", temp.Path
        };

        var result = CommandLineParser.TryParse(args, out var options, out var error);

        Assert.True(result);
        Assert.Null(error);
        Assert.NotNull(options);
        Assert.Equal("10.0.0.5", options!.IpAddress);
        Assert.Equal(Path.Combine(temp.Path, "settings.conf"), options.SettingsPath);
    }

    [Fact]
    public void TryParse_ReturnsFalse_ForInvalidPort()
    {
        var args = new[] { "--ip", "192.168.0.2", "--password", "secret", "--port", "invalid" };

        var result = CommandLineParser.TryParse(args, out var options, out var error);

        Assert.False(result);
        Assert.Null(options);
        Assert.Equal("Invalid SSH port 'invalid'.", error);
    }

    private sealed class TempDirectory : IDisposable
    {
        public TempDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "openipc-cli-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(Path))
                {
                    Directory.Delete(Path, recursive: true);
                }
            }
            catch
            {
                // Ignore cleanup failures in tests.
            }
        }
    }
}
