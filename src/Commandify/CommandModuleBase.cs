using Commandify.Abstractions.Types.Contexts;

namespace Commandify;

public class CommandModuleBase<TContext>
    where TContext : class, ICommandContext
{
    public TContext Context => _contextAccessor.Context;

    private readonly CommandContextAccessor<TContext> _contextAccessor;
    public CommandModuleBase(CommandContextAccessor<TContext> contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }
}