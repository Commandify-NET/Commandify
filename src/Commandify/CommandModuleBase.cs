using Commandify.Abstractions;
using Commandify.Abstractions.Execution;
using Commandify.Abstractions.Types.Contexts;

namespace Commandify;

public class CommandModuleBase<TContext> : ICommandModule<TContext>
    where TContext : class, ICommandContext
{
    public TContext Context => _contextAccessor.Context;
    
    private readonly ICommandContextAccessor<TContext> _contextAccessor;

    protected CommandModuleBase(ICommandContextAccessor<TContext> contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }
}