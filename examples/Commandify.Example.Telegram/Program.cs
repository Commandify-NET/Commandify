using Commandify;
using Commandify.Conversion.TypeReaders;
using Commandify.Example.Telegram;
using Commandify.Example.Telegram.Commands;
using Commandify.Example.Telegram.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

const string Token = "6251960488:AAG42svEhoBiab6kInx_p1pDcTR_NA6dx9c";

var builder = Host.CreateApplicationBuilder();

builder.Services.AddSingleton<CommandifyUpdateHandler>()
    .AddHostedService<TelegramService>()
    .AddSingleton<ITelegramBotClient>(new TelegramBotClient(Token))
    
    .AddTypeReaderPipeline(_ => _
        .UseReader<ConvertReader>())
    .AddCommandExecutor<TelegramMessageContext>(_ => _
        .UseModule<PingModule>());

await builder.Build().RunAsync();