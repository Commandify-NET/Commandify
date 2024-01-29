# Commandify


Commandify is a simple library which generates additional metadata to your class-based commands to simplify command execution

## Examples

Here are some basic examples how to use this library.

There's two types of command modules - Contextful and Contextless. Contextful commands are used when you need to access the context of the command, while Contextless commands aren't

### Contextless

Assume you have following simple command module:

```csharp
[CommandModule(Name = "ping")]
public class PingModule : CommandModuleBase
{
    [Command]
    public Task PingAsync()
    {
        Console.WriteLine("Pong!");
        
        return Task.CompletedTask;
    }
    
    public PingModule(ICommandContextAccessor<SampleContext> contextAccessor) : base(contextAccessor)
    {
    }
} 
```

To register this module to be available in your command executor, you need to use `AddCommandExecutor` extension method and register it manually:

```csharp
serviceCollection.AddCommandExecutor(executorBuilder =>
{
    return executorBuilder
        .UseModule<PingModule>();
});
```

And then execute it:
```csharp
var commandExecutor = serviceProvider.GetRequiredService<ICommandExecutor>();

await commandExecutor.ExecuteAsync("ping");
```

### Contextful

Approach is very similar to Contextless commands, but you also need to specify the context type in base class:

```csharp
public record HelpContext : ICommandContext
{
    public IServiceProvider Services { get; init; }
}

[CommandModule(Name = "help")]
public partial class HelpModule : CommandModuleBase<HelpContext>
{
    [Command]
    public Task HelpAsync()
    {
        var helpService = Context.Services.GetRequiredService<IHelpService>();
        
        Console.WriteLine(helpService.GetHelp());
        
        return Task.CompletedTask;
    }

    public HelpModule(ICommandContextAccessor<HelpContext> contextAccessor) : base(contextAccessor)
    {
    }
}
```

Registration and execution:

```csharp
serviceCollection.AddCommandExecutor<HelpContext>(_ => _
        .UseModule<HelpModule>())
    .AddTypeReaderPipeline(_ => _
        .UseReader<ConvertReader>());

var serviceProvider = serviceCollection.BuildServiceProvider();

var commandExecutor = serviceProvider.GetRequiredService<ICommandExecutor<HelpContext>>();

await commandExecutor.ExecuteAsync("help", new HelpContext() { Services = serviceProvider });
```

### Command module actions

Command module actions are used to execute your code before or after command execution. You can use them either by implementing `ICommandModuleActions` or inheriting from `CommandModuleBase`:

```csharp
public class SampleModule : CommandModuleBase
{
    public override Task OnActivatedAsync(CancellationToken cancellationToken)
    {
        return base.OnActivatedAsync(cancellationToken);
    }

    public override Task<bool> CheckPreConditionsAsync(CommandInfo commandInfo, CancellationToken cancellationToken)
    {
        return base.CheckPreConditionsAsync(commandInfo, cancellationToken);
    }

    public override Task OnAfterExecutionAsync(CommandInfo commandInfo, CancellationToken cancellationToken)
    {
        return base.OnAfterExecutionAsync(commandInfo, cancellationToken);
    }

    public override Task OnBeforeExecutionAsync(CommandInfo commandInfo, CancellationToken cancellationToken)
    {
        return base.OnBeforeExecutionAsync(commandInfo, cancellationToken);
    }
}
```