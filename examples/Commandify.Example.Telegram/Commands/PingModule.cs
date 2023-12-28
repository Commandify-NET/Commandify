using Commandify.Abstractions.Annotations;
using Commandify.Abstractions.Execution;
using Commandify.Example.Telegram.Contexts;

namespace Commandify.Example.Telegram.Commands;

[CommandModule(Name = "ping")]
public partial class PingModule : TelegramModuleBase
{
    public PingModule(ICommandContextAccessor<TelegramMessageContext> contextAccessor) : base(contextAccessor)
    {
    }
    
    public async Task Ping()
    {
        await ReplyAsync("Pong");
    }
}