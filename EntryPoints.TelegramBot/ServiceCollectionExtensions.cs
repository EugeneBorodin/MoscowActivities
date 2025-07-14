using EntryPoints.TelegramBot.BotCommands;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Utils.Settings;

namespace EntryPoints.TelegramBot;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTelegramBot(this IServiceCollection services, BotSettings botSettings)
    {
        services
            .AddHttpClient("TelegramBotClient")
            .AddTypedClient<ITelegramBotClient>((client, _) =>
                new TelegramBotClient(botSettings.ApiToken, client));
        
        services.AddTransient<IBotCommandFactory, BotCommandFactory>();
        
        services.AddTransient<IUpdateHandler, BotClientUpdateHandler>();
        services.AddHostedService<TelegramBotBackgroundService>();

        return services;
    }
}