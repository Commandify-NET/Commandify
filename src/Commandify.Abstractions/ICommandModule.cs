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