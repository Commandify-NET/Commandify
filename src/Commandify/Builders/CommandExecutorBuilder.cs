using System.Collections.Immutable;
using Commandify.Abstractions;
using Commandify.Abstractions.Conversion;
using Commandify.Abstractions.Execution;
using Commandify.Abstractions.Types;
using Commandify.Abstractions.Types.Contexts;
using Commandify.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Commandify.Builders;

public class CommandExecutorBuilder
{
    private readonly IServiceCollection _serviceCollection;
    private readonly ImmutableArray<CommandModuleInfo> _modules;

    private readonly Func<IServiceProvider, ITypeReaderPipeline> _typeReaderPipelineResolver;
    
    internal CommandExecutorBuilder(IServiceCollection serviceCollection, ImmutableArray<CommandModuleInfo>? modules, Func<IServiceProvider, ITypeReaderPipeline>? typeReaderPipelineResolver)
    {
        _serviceCollection = serviceCollection;
        _modules = modules ?? ImmutableArray<CommandModuleInfo>.Empty;
        _typeReaderPipelineResolver = typeReaderPipelineResolver ?? (static sp => sp.GetService<ITypeReaderPipeline>()!);
    }
    
    public CommandExecutorBuilder UseModule<TModule>(ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        where TModule : ICommandModule
    {
        _serviceCollection.TryAdd(ServiceDescriptor.Describe(typeof(TModule), typeof(TModule), serviceLifetime));
        
        var moduleInfo = TModule.ModuleInfo;

        return new CommandExecutorBuilder(_serviceCollection, _modules.Add(moduleInfo), _typeReaderPipelineResolver);
    }

    public CommandExecutorBuilder UseModule<TModule>(
        Func<CommandExecutorBuilder, CommandExecutorBuilder> configureChildren, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        where TModule : ICommandModule
    {
        _serviceCollection.TryAdd(ServiceDescriptor.Describe(typeof(TModule), typeof(TModule), serviceLifetime));
        
        var moduleInfo = TModule.ModuleInfo;
        
        var childrenModules = configureChildren(new CommandExecutorBuilder(_serviceCollection, ImmutableArray<CommandModuleInfo>.Empty, _typeReaderPipelineResolver));

        return new CommandExecutorBuilder(_serviceCollection, _modules.Add(moduleInfo with
        {
            ChildModules = childrenModules._modules
        }), _typeReaderPipelineResolver);
    }
    
    public CommandExecutorBuilder UseTypeReaderPipeline<TTypeReaderPipeline>()
        where TTypeReaderPipeline : ITypeReaderPipeline
    {
        return new CommandExecutorBuilder(_serviceCollection, _modules, _ => _.GetRequiredService<TTypeReaderPipeline>());
    }
    
    public CommandExecutorBuilder UseTypeReaderPipeline<TTypeReaderPipeline>(
        TTypeReaderPipeline typeReaderPipeline)
        where TTypeReaderPipeline : ITypeReaderPipeline
    {
        return new CommandExecutorBuilder(_serviceCollection, _modules, _ => typeReaderPipeline);
    }
    
    public CommandExecutorBuilder UseTypeReaderPipeline(
        Func<IServiceProvider, ITypeReaderPipeline> typeReaderPipelineResolver)
    {
        return new CommandExecutorBuilder(_serviceCollection, _modules, typeReaderPipelineResolver);
    }
    
    public ICommandExecutor Build(IServiceProvider serviceProvider)
    {
        var typeReaderPipeline = _typeReaderPipelineResolver(serviceProvider);

        if (typeReaderPipeline is null)
        {
            throw new InvalidOperationException("Type reader pipeline cannot be null");
        }
        
        return new CommandExecutor(_modules, serviceProvider, typeReaderPipeline);
    }
}


public class CommandExecutorBuilder<TContext>
    where TContext : class, ICommandContext
{
    private readonly IServiceCollection _serviceCollection;
    private readonly ImmutableArray<CommandModuleInfo> _modules;

    private readonly Func<IServiceProvider, ITypeReaderPipeline> _typeReaderPipelineResolver;
    
    internal CommandExecutorBuilder(IServiceCollection serviceCollection, ImmutableArray<CommandModuleInfo>? modules, Func<IServiceProvider, ITypeReaderPipeline>? typeReaderPipelineResolver)
    {
        _serviceCollection = serviceCollection;
        _modules = modules ?? ImmutableArray<CommandModuleInfo>.Empty;
        _typeReaderPipelineResolver = typeReaderPipelineResolver ?? (static sp => sp.GetService<ITypeReaderPipeline>()!);
    }
    
    public CommandExecutorBuilder<TContext> UseModule<TModule>(ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        where TModule : ICommandModule<TContext>, ICommandModule
    {
        _serviceCollection.TryAdd(ServiceDescriptor.Describe(typeof(TModule), typeof(TModule), serviceLifetime));
        
        var moduleInfo = TModule.ModuleInfo;

        return new CommandExecutorBuilder<TContext>(_serviceCollection, _modules.Add(moduleInfo), _typeReaderPipelineResolver);
    }

    public CommandExecutorBuilder<TContext> UseModule<TModule>(
        Func<CommandExecutorBuilder<TContext>, CommandExecutorBuilder<TContext>> configureChildren, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        where TModule : ICommandModule<TContext>, ICommandModule
    {
        _serviceCollection.TryAdd(ServiceDescriptor.Describe(typeof(TModule), typeof(TModule), serviceLifetime));
        
        var moduleInfo = TModule.ModuleInfo;
        
        var childrenModules = configureChildren(new CommandExecutorBuilder<TContext>(_serviceCollection, ImmutableArray<CommandModuleInfo>.Empty, _typeReaderPipelineResolver));

        return new CommandExecutorBuilder<TContext>(_serviceCollection, _modules.Add(moduleInfo with
        {
            ChildModules = childrenModules._modules
        }), _typeReaderPipelineResolver);
    }
    
    public CommandExecutorBuilder<TContext> UseTypeReaderPipeline<TTypeReaderPipeline>()
        where TTypeReaderPipeline : ITypeReaderPipeline
    {
        return new CommandExecutorBuilder<TContext>(_serviceCollection, _modules, _ => _.GetRequiredService<TTypeReaderPipeline>());
    }
    
    public CommandExecutorBuilder<TContext> UseTypeReaderPipeline<TTypeReaderPipeline>(
        TTypeReaderPipeline typeReaderPipeline)
        where TTypeReaderPipeline : ITypeReaderPipeline
    {
        return new CommandExecutorBuilder<TContext>(_serviceCollection, _modules, _ => typeReaderPipeline);
    }
    
    public CommandExecutorBuilder<TContext> UseTypeReaderPipeline(
        Func<IServiceProvider, ITypeReaderPipeline> typeReaderPipelineResolver)
    {
        return new CommandExecutorBuilder<TContext>(_serviceCollection, _modules, typeReaderPipelineResolver);
    }
    
    public ICommandExecutor<TContext> Build(IServiceProvider serviceProvider)
    {
        var typeReaderPipeline = _typeReaderPipelineResolver(serviceProvider);

        if (typeReaderPipeline is null)
        {
            throw new InvalidOperationException("Type reader pipeline cannot be null");
        }

        var contextAccessor = serviceProvider.GetRequiredService<ICommandContextAccessor<TContext>>();
        
        if(contextAccessor is null)
        {
            throw new InvalidOperationException("Context accessor cannot be null");
        }
        
        var internalExecutor = new CommandExecutor(_modules, serviceProvider, typeReaderPipeline);

        return new CommandExecutor<TContext>(internalExecutor, contextAccessor);
    }
}