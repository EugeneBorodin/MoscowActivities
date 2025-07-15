using System.Text.RegularExpressions;
using MediatR;
using Microsoft.Extensions.Logging;
using MoscowActivityServices.Abstractions.Models;
using Telegram.Bot.Types;
using UseCases.Handlers;

namespace EntryPoints.TelegramBot.BotCommands.Messages;

public class UpdateConfigBotCommand : IBotCommand
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
            var cfg = ExtractUserConfigFromText(message);

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

    public static UserConfig ExtractUserConfigFromText(Message message)
    {
        var config = new UserConfig
        {
            Username = message.Chat.Username
        };
        
        var text = message.Text;

        // Extract user info
        config.Fullname = Regex.Match(text, @"Имя:\s*(.+)").Groups[1].Value.Trim();
        config.PhoneNumber = Regex.Match(text, @"Телефон:\s*(\+?\d+)").Groups[1].Value.Trim();
        config.Email = Regex.Match(text, @"Email:\s*(\S+@\S+)").Groups[1].Value.Trim();

        // Extract slot params
        var slotPattern = new Regex(@"(?<day>[А-Яа-я]+),\s*(?<time>\d{1,2}:\d{2}),\s*(?<count>\d+)\s*м",
            RegexOptions.Multiline);
        
        foreach (Match match in slotPattern.Matches(text))
        {
            var dayName = match.Groups["day"].Value.Trim();
            var timeStr = match.Groups["time"].Value.Trim();
            var peopleCount = int.Parse(match.Groups["count"].Value);

            if (TryParseRussianDay(dayName, out DayOfWeek dayOfWeek) &&
                TimeOnly.TryParseExact(timeStr, "HH:mm", out TimeOnly time))
            {
                config.SlotParams.Add(new SlotParam
                {
                    DayOfWeek = dayOfWeek,
                    Time = time,
                    PeopleCount = peopleCount
                });
            }
        }

        return config;
    }

    private static bool TryParseRussianDay(string russianDay, out DayOfWeek dayOfWeek)
    {
        // Simple mapping from Russian to DayOfWeek enum
        var dayMap = new Dictionary<string, DayOfWeek>(StringComparer.OrdinalIgnoreCase)
        {
            ["Понедельник"] = DayOfWeek.Monday,
            ["Вторник"] = DayOfWeek.Tuesday,
            ["Среда"] = DayOfWeek.Wednesday,
            ["Четверг"] = DayOfWeek.Thursday,
            ["Пятница"] = DayOfWeek.Friday,
            ["Суббота"] = DayOfWeek.Saturday,
            ["Воскресенье"] = DayOfWeek.Sunday,
        };

        return dayMap.TryGetValue(russianDay, out dayOfWeek);
    }
}