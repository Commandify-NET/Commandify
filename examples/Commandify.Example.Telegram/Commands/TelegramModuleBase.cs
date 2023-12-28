using Commandify.Abstractions;
using Commandify.Abstractions.Execution;
using Commandify.Example.Telegram.Contexts;
using Telegram.Bot;

namespace Commandify.Example.Telegram.Commands;

public abstract class TelegramModuleBase : ICommandModule<TelegramMessageContext>
{
    private readonly ICommandContextAccessor<TelegramMessageContext> _contextAccessor;
    public TelegramMessageContext Context => _contextAccessor.Context;

    protected TelegramModuleBase(ICommandContextAccessor<TelegramMessageContext> contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    protected Task SendAsync(string text)
    {
        return Context.BotClient.SendTextMessageAsync(Context.Message.Chat, text);
    }
    
    protected Task ReplyAsync(string text)
    {
        return Context.BotClient.SendTextMessageAsync(Context.Message.Chat, text, replyToMessageId: Context.Message.MessageId);
    }
}