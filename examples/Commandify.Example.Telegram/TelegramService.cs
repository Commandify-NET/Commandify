using Microsoft.Extensions.Hosting;
using Telegram.Bot;

public class TelegramService : BackgroundService
{
    private readonly CommandifyUpdateHandler _commandifyUpdateHandler;
    private readonly ITelegramBotClient _telegramBotClient;

    public TelegramService(CommandifyUpdateHandler commandifyUpdateHandler, ITelegramBotClient telegramBotClient)
    {
        _commandifyUpdateHandler = commandifyUpdateHandler;
        _telegramBotClient = telegramBotClient;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => _telegramBotClient.ReceiveAsync(_commandifyUpdateHandler, cancellationToken: stoppingToken);
}