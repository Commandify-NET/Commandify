using System.Collections.Immutable;
using Commandify.Abstractions.Conversion;
using Commandify.Abstractions.Execution;
using Commandify.Abstractions.Types;
using Commandify.Abstractions.Types.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace Commandify.Execution;

public class CommandExecutor : ICommandExecutor
{
    private readonly ReadOnlyMemory<CommandModuleInfo> _commandModules;
    private readonly IServiceProvider _serviceProvider;
    private readonly ITypeReaderPipeline _typeReaderPipeline;

    public CommandExecutor(ImmutableArray<CommandModuleInfo> commandModules, IServiceProvider serviceProvider, ITypeReaderPipeline typeReaderPipeline)
    {
        _commandModules = commandModules.AsMemory();
        _serviceProvider = serviceProvider;
        _typeReaderPipeline = typeReaderPipeline;
    }
    
    public Task ExecuteAsync(
        ReadOnlySpan<char> text)
    {
        var commandMatch = CommandMatcher.GetMatch(text, _commandModules);

        if (!commandMatch.Matched)
            return Task.CompletedTask;

        var module = commandMatch.Module!.Value;
        var command = commandMatch.Command!.Value;
        
        var parseResult = CommandArgumentsParser.Parse(commandMatch.Text, command.Parameters, _typeReaderPipeline);
        
        if (!parseResult.Success)
            return Task.CompletedTask;

        object? moduleInstance = null!;
        
        if (!command.Method.IsStatic)
        {
            moduleInstance = _serviceProvider.GetRequiredService(module.Type);
        }

        var commandExecuteResult = command.Method.Invoke(moduleInstance, parseResult.Arguments.ToArray());

        if (commandExecuteResult is not Task commandTask)
        {
            throw new InvalidOperationException("Cannot execute command");
        }

        return commandTask;
    }
}

public class CommandExecutor<TContext> : ICommandExecutor<TContext>
    where TContext : class, ICommandContext
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly ICommandContextAccessor<TContext> _contextAccessor;

    public CommandExecutor(ICommandExecutor commandExecutor, ICommandContextAccessor<TContext> contextAccessor)
    {
        _commandExecutor = commandExecutor;
        _contextAccessor = contextAccessor;
    }

    public Task ExecuteAsync(ReadOnlySpan<char> text, TContext context)
    {
        _contextAccessor.Context = context;
        
        return _commandExecutor.ExecuteAsync(text)
            .ContinueWith(_ => _contextAccessor.Context = null!);
    }
}