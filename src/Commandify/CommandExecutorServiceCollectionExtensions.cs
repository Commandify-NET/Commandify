using System.Collections.Immutable;
using Commandify.Abstractions;
using Commandify.Abstractions.Builders;
using Commandify.Abstractions.Conversion;
using Commandify.Abstractions.Types;
using Commandify.Builders;
using Commandify.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace Commandify;

public static class CommandExecutorServiceCollectionExtensions
{
    public static IServiceCollection AddCommandExecutor(this IServiceCollection serviceCollection, Func<CommandExecutorBuilder, CommandExecutorBuilder> configureCommandExecutor, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        serviceCollection.AddSingleton(typeof(CommandContextAccessor<>));
        
        var commandExecutorBuilder = configureCommandExecutor(new CommandExecutorBuilder(serviceCollection, ImmutableArray<CommandModuleInfo>.Empty, null));

        serviceCollection.Add(ServiceDescriptor.Describe(typeof(CommandExecutor),
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