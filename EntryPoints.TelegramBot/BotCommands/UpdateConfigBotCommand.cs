using MediatR;
using Microsoft.Extensions.Logging;
using MoscowActivityServices.Abstractions.Models;
using Telegram.Bot.Types;
using UseCases.Handlers;

namespace EntryPoints.TelegramBot.BotCommands;

public class UpdateConfigBotCommand: IBotCommand
{
    private readonly IMediator _mediator;
    private readonly ILogger<UpdateConfigBotCommand> _logger;

    public UpdateConfigBotCommand(IMediator mediator, ILogger<UpdateConfigBotCommand> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    public async Task<string> Execute(Message message)
    {
        string response;
        
        try
        {
            var cfg = ExtractUserConfig(message);

            await _mediator.Send(new UpdateUserBookingConfigRequest
            {
                UserConfig = cfg,
            });

            response = "Новая конфигурация сохранена";
        }
        catch (Exception e)
        {
            response =
                $"При попытке задать новую конфигурацию для пользователя {message.Chat.Username} произошла ошибка";
            
            _logger.LogError(e, response);
        }

        return response;
    }

    private UserConfig ExtractUserConfig(Message message)
    {
        var userConfig = new UserConfig();
        // TODO
        return userConfig;
    }
}