using Commandify.Abstractions.Types.Contexts;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Commandify.Example.Telegram.Contexts;

public class TelegramMessageContext : ICommandContext
{
    public IServiceProvider Services { get; init; }
    
    public Message Message { get; init; }
    
    public ITelegramBotClient BotClient { get; init; }
}