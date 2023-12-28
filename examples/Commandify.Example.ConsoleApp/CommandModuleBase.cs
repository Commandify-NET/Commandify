using Commandify.Abstractions.Execution;

namespace Commandify.Example.ConsoleApp;

public abstract class CommandModuleBase : CommandModuleBase<SampleContext>
{
    protected CommandModuleBase(ICommandContextAccessor<SampleContext> contextAccessor) : base(contextAccessor)
    {
    }
}