using Commandify.Abstractions;
using Commandify.Abstractions.Execution;
using Commandify.Abstractions.Types;
using Commandify.Abstractions.Types.Contexts;

namespace Commandify;

public abstract class CommandModuleBase<TContext> : ICommandModule<TContext>, ICommandModuleActions
    where TContext : class, ICommandContext
{
    public TContext Context => _contextAccessor.Context;
    
    private readonly ICommandContextAccessor<TContext> _contextAccessor;

    protected CommandModuleBase(ICommandContextAccessor<TContext> contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }
    
    /// <summary>
    /// Called when new instance of module was created/retrieved from DI.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task OnActivatedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    
    /// <summary>
    /// Before command execution.
    /// </summary>
    /// <param name="commandInfo"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task OnBeforeExecutionAsync(CommandInfo commandInfo, CancellationToken cancellationToken) => Task.CompletedTask;
    
    public virtual Task OnAfterExecutionAsync(CommandInfo commandInfo, CancellationToken cancellationToken) => Task.CompletedTask;
    
    public virtual Task<bool> CheckPreConditionsAsync(CommandInfo commandInfo, CancellationToken cancellationToken) => Task.FromResult(true);
}