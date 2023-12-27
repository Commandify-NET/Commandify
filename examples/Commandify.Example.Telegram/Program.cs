// See https://aka.ms/new-console-template for more information

using Commandify;
using Commandify.Abstractions.Execution;
using Commandify.Conversion.TypeReaders;
using Commandify.Example.Telegram.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

const string Token = "6251960488:AAH1-KGMLV-vnLW-H5zeMVg40EqVUjkNFz8";

var builder = Host.CreateApplicationBuilder();

builder.Services.AddSingleton<CommandifyUpdateHandler>()
    .AddHostedService<TelegramService>()
    .AddSingleton<ITelegramBotClient>(new TelegramBotClient(Token))
    
    .AddTypeReaderPipeline(_ => _
        .UseReader<ConvertReader>())
    .AddCommandExecutor(_ => _
        .UseModule<PingModule>());

await builder.Build().RunAsync();


public class CommandifyUpdateHandler : IUpdateHandler
{
    private readonly ICommandExecutor _commandExecutor;

    public CommandifyUpdateHandler(ICommandExecutor commandExecutor, IServiceProvider serviceProvider)
    {
        _commandExecutor = commandExecutor;
    }
    
    public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        return update switch
        {
            { Message: { Text: {} text } } => _commandExecutor.ExecuteAsync(text),
            
            _ => Task.CompletedTask
        };
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}