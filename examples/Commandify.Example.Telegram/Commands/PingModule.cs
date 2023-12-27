using Commandify.Abstractions.Annotations;
using Commandify.Example.Telegram.Contexts;
using Telegram.Bot;

namespace Commandify.Example.Telegram.Commands;

[CommandModule(Name = "ping")]
public partial class PingModule : CommandModuleBase<TelegramMessageContext>
{
    public async Task Ping()
    {
        await Context.BotClient.SendTextMessageAsync(Context.Message.Chat, "Pong!");
    }
}