// See https://aka.ms/new-console-template for more information

using Commandify;
using Commandify.Abstractions.Execution;
using Commandify.Conversion.TypeReaders;
using Commandify.Example.Telegram;
using Commandify.Example.Telegram.Commands;
using Commandify.Example.Telegram.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

const string Token = "<YOUR BOT API TOKEN HERE>";

var builder = Host.CreateApplicationBuilder();

builder.Services.AddSingleton<CommandifyUpdateHandler>()
    .AddHostedService<TelegramService>()
    .AddSingleton<ITelegramBotClient>(new TelegramBotClient(Token))
    
    .AddTypeReaderPipeline(_ => _
        .UseReader<ConvertReader>())
    .AddCommandExecutor<TelegramMessageContext>(_ => _
        .UseModule<PingModule>());

await builder.Build().RunAsync();


namespace Commandify.Example.Telegram
{
    public class CommandifyUpdateHandler : IUpdateHandler
    {
        private readonly ICommandExecutor<TelegramMessageContext> _commandExecutor;
        private readonly IServiceProvider _serviceProvider;

        public CommandifyUpdateHandler(ICommandExecutor<TelegramMessageContext> commandExecutor, IServiceProvider serviceProvider)
        {
            _commandExecutor = commandExecutor;
            _serviceProvider = serviceProvider;
        }
    
        public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            return update switch
            {
                { Message: { Text: {} text } message } => _commandExecutor.ExecuteAsync(text, new TelegramMessageContext()
                {
                    Message = message,
                    BotClient = botClient,
                    Services = _serviceProvider
                }),
            
                _ => Task.CompletedTask
            };
        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}