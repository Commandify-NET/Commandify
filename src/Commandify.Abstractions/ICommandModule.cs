using Commandify.Abstractions.Types;
using Commandify.Abstractions.Types.Contexts;

namespace Commandify.Abstractions;

public interface ICommandModule<TContext> 
    where TContext : class, ICommandContext
{
    TContext Context { get; }
}

public interface ICommandModule
{
    static abstract string Name { get; }
    
    static abstract CommandModuleInfo ModuleInfo { get; }
}

public interface ICommandModuleActions
{
    /// <summary>
    /// Called when new instance of module was created/retrieved from DI.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task OnActivatedAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Before command execution.
    /// </summary>
    /// <param name="commandInfo"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task OnBeforeExecutionAsync(CommandInfo commandInfo, CancellationToken cancellationToken);
    
    Task OnAfterExecutionAsync(CommandInfo commandInfo, CancellationToken cancellationToken);
    
    Task<bool> CheckPreConditionsAsync(CommandInfo commandInfo, CancellationToken cancellationToken);
}