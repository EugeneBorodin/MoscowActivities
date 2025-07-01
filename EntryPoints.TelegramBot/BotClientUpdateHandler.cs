using System.Text;
using Microsoft.Extensions.Logging;
using MoscowActivityServices.Abstractions;
using MoscowActivityServices.Abstractions.Models;
using NodaTime;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

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

        string answer = string.Empty;
        Slot[] slots = new Slot[] { };
        
        try
        {
            slots = await GetInformationAboutSlots();
        }
        catch (Exception e)
        {
            var errMessage = "При получении информации о свободных слотах произошла ошибка";
            _logger.LogError(e, errMessage);
            
            answer = errMessage + ". Обратитесь к администратору бота.";
            await botClient.SendMessage(update.Message.Chat.Id, answer, cancellationToken: cancellationToken);
        }
        
        if (!slots.Any())
        {
            answer = "Извините, слотов не найдено";
            await botClient.SendMessage(update.Message.Chat.Id, answer, cancellationToken: cancellationToken);
        }

        foreach (var slot in slots)
        {
            var message = GenerateAnswer(slot);
            await botClient.SendMessage(update.Message.Chat.Id, message, cancellationToken: cancellationToken);
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _logger.LogError(exception.Message, exception);
        return Task.FromException(exception);
    }

    private async Task<Slot[]> GetInformationAboutSlots()
    {
        var today = LocalDate.FromDateTime(DateTime.Now);
        var searchRequest = new SearchRequest
        {
            From = today,
            Till = LocalDate.Add(today, Period.FromDays(1)),
            CompanyId = 1318073
        };

        var slotsResponse = await _activityService.FindSlots(searchRequest);
        return slotsResponse as Slot[] ?? slotsResponse.ToArray();
    }

    private string GenerateAnswer(Slot slot)
    {
        var sb = new StringBuilder();
        sb.AppendLine(slot.Title);
        sb.AppendLine(slot.Location);
        sb.AppendLine(slot.Specialization);
        sb.AppendLine($"Продолжительность: {slot.Duration.TotalMinutes} мин");
        sb.AppendLine($"Осталось мест: {slot.Count}");
        sb.AppendLine();
        
        return sb.ToString();
    }
}