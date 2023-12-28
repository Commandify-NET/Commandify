using Commandify.Abstractions.Types.Contexts;

namespace Commandify.Example.ConsoleApp;

public class SampleContext : ICommandContext
{
    public IServiceProvider Services { get; init; }
}