using System.Collections.Immutable;
using Commandify.Abstractions;
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

    public CommandExecutor(ImmutableArray<CommandModuleInfo> commandModules, IServiceProvider serviceProvider,
        ITypeReaderPipeline typeReaderPipeline)
    {
        _commandModules = commandModules.AsMemory();
        _serviceProvider = serviceProvider;
        _typeReaderPipeline = typeReaderPipeline;
    }

    public Task ExecuteAsync(
        ReadOnlySpan<char> text, CancellationToken cancellationToken = default)
    {
        var commandMatch = CommandMatcher.GetMatch(text, _commandModules);

        if (!commandMatch.Matched)
            return Task.CompletedTask;

        var module = commandMatch.Module!.Value;
        var command = commandMatch.Command!.Value;

        var parseResult = CommandArgumentsParser.Parse(commandMatch.Text, command.Parameters, _typeReaderPipeline);

        if (!parseResult.Success)
            return Task.CompletedTask;

        return ExecuteAsync(commandMatch.Command.Value);

        async Task ExecuteAsync(CommandInfo commandInfo)
        {
            ICommandModule? moduleInstance = null!;

            if (!commandInfo.Method.IsStatic)
            {
                moduleInstance = (ICommandModule)_serviceProvider.GetRequiredService(module.Type);
            }
            
            var moduleActions = moduleInstance as ICommandModuleActions;

            await (moduleActions?.OnActivatedAsync(cancellationToken) ?? Task.CompletedTask);
            
            bool preConditionsResult = await (moduleActions?.CheckPreConditionsAsync(commandInfo, cancellationToken) ?? Task.FromResult(true));
            
            if (!preConditionsResult)
                return;

            await (moduleActions?.OnBeforeExecutionAsync(commandInfo, cancellationToken) ?? Task.CompletedTask);

            var commandExecuteResult = commandInfo.Method.Invoke(moduleInstance, parseResult.Arguments.ToArray());

            await (moduleActions?.OnAfterExecutionAsync(commandInfo, cancellationToken) ?? Task.CompletedTask);

            if (commandExecuteResult is not Task commandTask)
            {
                throw new InvalidOperationException("Cannot execute command");
            }

            await commandTask;
        }
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

    public Task ExecuteAsync(ReadOnlySpan<char> text, TContext context, CancellationToken cancellationToken = default)
    {
        _contextAccessor.Context = context;

        return _commandExecutor.ExecuteAsync(text, cancellationToken)
            .ContinueWith(_ => _contextAccessor.Context = null!, cancellationToken);
    }
}