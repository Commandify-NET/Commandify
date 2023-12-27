using Commandify.Abstractions;
using Commandify.Abstractions.Execution;
using Commandify.Abstractions.Types.Contexts;

namespace Commandify;

public class CommandContextAccessor<TContext> : ICommandContextAccessor<TContext> where TContext : class, ICommandContext
{
    private static readonly AsyncLocal<CommandContextHolder> CommandContextCurrent = new();

    public TContext Context
    {
        get => CommandContextCurrent.Value?.Context!;
        set
        {
            var holder = CommandContextCurrent.Value;
            if (holder != null)
            {
                holder.Context = null!;
            }

            if (value != null!)
            {
                CommandContextCurrent.Value = new CommandContextHolder { Context = value };
            }
        }
    }

    private sealed class CommandContextHolder
    {
        public TContext Context;
    }
}