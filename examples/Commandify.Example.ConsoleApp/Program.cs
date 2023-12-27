using Commandify;
using Commandify.Conversion.TypeReaders;
using Commandify.Example.ConsoleApp;
using Commandify.Execution;
using Microsoft.Extensions.DependencyInjection;

IServiceCollection serviceCollection = new ServiceCollection();

serviceCollection.AddCommandExecutor(_ => _
    .UseModule<HelpModule>())
    
    .AddTypeReaderPipeline(_ => _
        .UseReader<ConvertReader>());

var serviceProvider = serviceCollection.BuildServiceProvider();

var commandExecutor = serviceProvider.GetRequiredService<CommandExecutor>();

await commandExecutor.ExecuteAsync("help");