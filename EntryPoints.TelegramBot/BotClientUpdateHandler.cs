using System.Text.Json;
using Microsoft.Extensions.Logging;
using MoscowActivityServices.Abstractions;
using MoscowActivityServices.Abstractions.Models;
using NodaTime;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EntryPoints.TelegramBot;

public class BotClientUpdateHandler : IUpdateHandler
{
    private readonly ILogger<BotClientUpdateHandler> _logger;
    
    private readonly IActivityService _activityService;
    
    public BotClientUpdateHandler(ILogger<BotClientUpdateHandler> logger, IActivityService activityService)
    {
        _logger = logger;
        _activityService = activityService;
    }

    public async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _logger.LogInformation($"Update received {update.Message.Text}");

        var answer = await GetInformationAboutSlots();
        
        await botClient.SendMessage(update.Message.Chat.Id, answer, cancellationToken: cancellationToken);
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _logger.LogError(exception.Message, exception);
        return Task.FromException(exception);
    }

    private async Task<string> GetInformationAboutSlots()
    {
        var today = LocalDate.FromDateTime(DateTime.Now);
        var searchRequest = new SearchRequest
        {
            From = today,
            Till = LocalDate.Add(today, Period.FromDays(7)),
            CompanyId = 1367196,
            Count = 100
        };

        var slots = await _activityService.FindSlots(searchRequest);

        var scheduleData = slots as ScheduleData[] ?? slots.ToArray();
        
        if (!scheduleData.Any())
        {
            return "Извините, слотов не найдено";
        }
        
        return JsonSerializer.Serialize(scheduleData);
    }
}