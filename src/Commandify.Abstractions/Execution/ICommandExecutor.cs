namespace Commandify.Abstractions.Execution;

public interface ICommandExecutor
{
    Task ExecuteAsync(
        ReadOnlySpan<char> text);
}