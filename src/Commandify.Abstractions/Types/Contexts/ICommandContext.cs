namespace Commandify.Abstractions.Types.Contexts;

public interface ICommandContext
{
    IServiceProvider Services { get; }
}