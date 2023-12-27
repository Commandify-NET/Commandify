using Commandify.Abstractions.Annotations;
using Commandify.Abstractions.Types.Contexts;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Commandify.Example.Telegram.Commands;

public class TelegramMessageContext : ICommandContext
{
    public IServiceProvider Services { get; init; }
    
    public Message Message { get; init; }
    
    public ITelegramBotClient BotClient { get; init; }
}

[CommandModule(Name = "ping")]
public partial class PingModule : CommandModuleBase<TelegramMessageContext>
{
    public async Task Ping()
    {
        await Context.BotClient.SendTextMessageAsync(Context.Message.Chat, "Pong!");
    }
}