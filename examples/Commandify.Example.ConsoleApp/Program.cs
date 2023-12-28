using Commandify;
using Commandify.Abstractions.Execution;
using Commandify.Conversion.TypeReaders;
using Commandify.Example.ConsoleApp;
using Microsoft.Extensions.DependencyInjection;

IServiceCollection serviceCollection = new ServiceCollection();

serviceCollection.AddCommandExecutor<SampleContext>(_ => _
    .UseModule<HelpModule>())
    
    .AddTypeReaderPipeline(_ => _
        .UseReader<ConvertReader>());

var serviceProvider = serviceCollection.BuildServiceProvider();

var commandExecutor = serviceProvider.GetRequiredService<ICommandExecutor<SampleContext>>();

await commandExecutor.ExecuteAsync("help", new SampleContext
{
    Services = serviceProvider
});