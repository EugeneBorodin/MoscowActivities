using EntryPoints.TelegramBot.BotCommands.Messages;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EntryPoints.TelegramBot.BotCommands;

public class BotCommandFactory : IBotCommandFactory
{
    private readonly IServiceProvider _serviceProvider;
    
    public BotCommandFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public IBotCommand GetCommand(BotCommandType botCommandType)
    {
        switch (botCommandType)
        {
            case BotCommandType.UpdateUserConfig:
                return new UpdateConfigBotCommand(_serviceProvider.GetRequiredService<IMediator>(),
                    _serviceProvider.GetRequiredService<ILogger<UpdateConfigBotCommand>>());
            case BotCommandType.GetUserConfig:
                return new GetConfigBotCommand(_serviceProvider.GetRequiredService<IMediator>(),
                    _serviceProvider.GetRequiredService<ILogger<GetConfigBotCommand>>());
            case BotCommandType.GetHelp:
                return new GetHelpBotCommand(_serviceProvider.GetRequiredService<IMediator>(),
                    _serviceProvider.GetRequiredService<ILogger<GetHelpBotCommand>>());
            default: throw new ArgumentException("Команда не задана, воспользуйся справкой: /справка");
        }
    }
}