using Commandify.Abstractions.Types.Contexts;

namespace Commandify.Abstractions.Execution;

public interface ICommandExecutor
{
    Task ExecuteAsync(
        ReadOnlySpan<char> text);
}

public interface ICommandExecutor<TContext>
    where TContext : class, ICommandContext
{
    Task ExecuteAsync(
        ReadOnlySpan<char> text, TContext context);
}