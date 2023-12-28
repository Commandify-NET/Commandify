using System.Collections.Immutable;
using Commandify.Abstractions.Builders;
using Commandify.Abstractions.Conversion;
using Commandify.Abstractions.Execution;
using Commandify.Abstractions.Types;
using Commandify.Abstractions.Types.Contexts;
using Commandify.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace Commandify;

public static class CommandExecutorServiceCollectionExtensions
{
    public static IServiceCollection AddCommandExecutor(this IServiceCollection serviceCollection, Func<CommandExecutorBuilder, CommandExecutorBuilder> configureCommandExecutor, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        var commandExecutorBuilder = configureCommandExecutor(new CommandExecutorBuilder(serviceCollection, ImmutableArray<CommandModuleInfo>.Empty, null));
        
        serviceCollection.Add(ServiceDescriptor.Describe(typeof(ICommandExecutor),
            sp => commandExecutorBuilder.Build(sp), serviceLifetime));

        return serviceCollection;
    }
    
    public static IServiceCollection AddCommandExecutor<TContext>(this IServiceCollection serviceCollection, Func<CommandExecutorBuilder<TContext>, CommandExecutorBuilder<TContext>> configureCommandExecutor, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton) where TContext : class, ICommandContext
    {
        serviceCollection.AddSingleton(typeof(ICommandContextAccessor<>),typeof(CommandContextAccessor<>));
        
        var commandExecutorBuilder = configureCommandExecutor(new CommandExecutorBuilder<TContext>(serviceCollection, ImmutableArray<CommandModuleInfo>.Empty, null));
        
        serviceCollection.Add(ServiceDescriptor.Describe(typeof(ICommandExecutor<TContext>),
            sp => commandExecutorBuilder.Build(sp), serviceLifetime));

        return serviceCollection;
    }

    public static IServiceCollection AddTypeReaderPipeline(this IServiceCollection serviceCollection, Func<ITypeReaderPipelineBuilder, ITypeReaderPipelineBuilder> configure, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        var builder = configure(new TypeReaderPipelineBuilder(serviceCollection));

        serviceCollection.Add(ServiceDescriptor.Describe(typeof(ITypeReaderPipeline), sp => builder.Build(sp), serviceLifetime));

        return serviceCollection;
    }
}