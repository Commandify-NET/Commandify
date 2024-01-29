using Commandify.Abstractions.Execution;

namespace Commandify.Example.ConsoleApp;


[CommandModule(Name = "help")]
public partial class HelpModule : CommandModuleBase<SampleContext>
{
    [Command]
    public Task HelpAsync()
    {
        return Task.CompletedTask;
    }

    public HelpModule(ICommandContextAccessor<SampleContext> contextAccessor) : base(contextAccessor)
    {
    }
}