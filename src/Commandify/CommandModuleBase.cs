using Commandify.Abstractions.Execution;
using Commandify.Abstractions.Types.Contexts;

namespace Commandify;

public abstract class CommandModuleBase<TContext>
    where TContext : class, ICommandContext
{
    public TContext Context => _contextAccessor.Context;

    private readonly ICommandContextAccessor<TContext> _contextAccessor;
    
    public CommandModuleBase(ICommandContextAccessor<TContext> contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }
}