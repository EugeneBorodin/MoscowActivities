using System.Text;
using MediatR;
using Microsoft.Extensions.Logging;
using MoscowActivityServices.Abstractions.Models;
using Telegram.Bot.Types;
using UseCases.Handlers;

namespace EntryPoints.TelegramBot.BotCommands.Messages;

public class GetConfigBotCommand : IBotCommand
{
    private readonly IMediator _mediator;
    private readonly ILogger<GetConfigBotCommand> _logger;

    public GetConfigBotCommand(IMediator mediator, ILogger<GetConfigBotCommand> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    public async Task<string> Execute(Message message)
    {
        string response;
        
        try
        {
            var userConfig = await _mediator.Send(new GetUserBookingConfigRequest
            {
                Username = message.Chat.Username,
            });

            response = GenerateUserConfigMessage(userConfig);
        }
        catch (Exception e)
        {
            response = $"Попытка получить конфигурацию пользователя {message.Chat.Username} завершилась ошибкой";
            _logger.LogError(e, response);
        }
        
        return response;
    }

    private string GenerateUserConfigMessage(UserConfig config)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Имя: {config.Fullname}");
        sb.AppendLine($"Телефон: {config.PhoneNumber}");
        sb.AppendLine($"Email: {config.Email}");
        sb.AppendLine();
        sb.AppendLine("Слоты:");

        foreach (var slot in config.SlotParams)
        {
            string dayInRussian = GetRussianDayName(slot.DayOfWeek);
            string time = slot.Time.ToString("HH:mm");
            sb.AppendLine($"{dayInRussian}, {time}, {slot.PeopleCount} м");
        }

        return sb.ToString();
    }

    private static string GetRussianDayName(DayOfWeek day)
    {
        return day switch
        {
            DayOfWeek.Monday => "Понедельник",
            DayOfWeek.Tuesday => "Вторник",
            DayOfWeek.Wednesday => "Среда",
            DayOfWeek.Thursday => "Четверг",
            DayOfWeek.Friday => "Пятница",
            DayOfWeek.Saturday => "Суббота",
            DayOfWeek.Sunday => "Воскресенье",
            _ => ""
        };
    }
}