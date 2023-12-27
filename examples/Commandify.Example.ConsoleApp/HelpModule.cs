namespace Commandify.Example.ConsoleApp;

[CommandModule(Name = "help")]
public partial class HelpModule
{
    [Command]
    public Task HelpAsync()
    {
        return Task.CompletedTask;
    }
}