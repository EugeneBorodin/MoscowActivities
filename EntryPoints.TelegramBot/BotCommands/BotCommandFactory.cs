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
    
    public IBotCommand GetCommand(string message)
    {
        var data = message.Trim();

        if (data.StartsWith("/конфигурация"))
        {
            return new UpdateConfigBotCommand(_serviceProvider.GetRequiredService<IMediator>(),
                _serviceProvider.GetRequiredService<ILogger<UpdateConfigBotCommand>>());
        }
        
        throw new ArgumentException("Команда не задана");
    }
}