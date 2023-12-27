using Commandify.Abstractions.Types;

namespace Commandify.Types.Results;

public ref struct CommandMatchResult
{
    public bool Matched { get; init; }
    
    public CommandModuleInfo? Module { get; init; }
    
    public CommandInfo? Command { get; init; }
    
    public ReadOnlySpan<char> Text { get; init; }

    public CommandMatchResult()
    {
        Matched = false;
        Module = null;
        Command = null;
        Text = ReadOnlySpan<char>.Empty;
    }
    
    public CommandMatchResult(CommandModuleInfo module, CommandInfo command, ReadOnlySpan<char> text)
    {
        Matched = true;
        Module = module;
        Command = command;
        Text = text;
    }
}