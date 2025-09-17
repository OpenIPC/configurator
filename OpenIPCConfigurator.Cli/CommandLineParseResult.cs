namespace OpenIPCConfigurator.Cli;

internal sealed class CommandLineParseResult
{
    public CommandLineParseResult(CommandOptions options, CommandArguments arguments)
    {
        Options = options;
        Arguments = arguments;
    }

    public CommandOptions Options { get; }

    public CommandArguments Arguments { get; }
}
