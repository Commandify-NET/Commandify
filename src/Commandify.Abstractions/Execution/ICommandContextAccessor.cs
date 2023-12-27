using Commandify.Abstractions.Types.Contexts;

namespace Commandify.Abstractions.Execution;

public interface ICommandContextAccessor<TContext> where TContext : class, ICommandContext
{
    TContext Context { get; set; }
}