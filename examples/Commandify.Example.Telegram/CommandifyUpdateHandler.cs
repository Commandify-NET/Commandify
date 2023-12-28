using Commandify.Abstractions.Execution;
using Commandify.Example.Telegram.Contexts;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Commandify.Example.Telegram;

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