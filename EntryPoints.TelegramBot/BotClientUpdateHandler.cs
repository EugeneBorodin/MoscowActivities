using System.Text;
using Microsoft.Extensions.Caching.Memory;
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
    private readonly IMemoryCache _cache;

    public BotClientUpdateHandler(ILogger<BotClientUpdateHandler> logger, IActivityService activityService, IMemoryCache cache)
    {
        _logger = logger;
        _activityService = activityService;
        _cache = cache;
    }

    public async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (update.ChannelPost == null)
        {
            return;
        }

        _logger.LogInformation(
            $"Update received: {update.ChannelPost.Text} in chat with id: {update.ChannelPost.Chat.Id}");

        string answer = string.Empty;
        var slots = new List<Slot>();
        
        var today = LocalDate.FromDateTime(DateTime.Now);
        var searchRequest = new SearchRequest
        {
            From = today,
            Till = LocalDate.Add(today, Period.FromDays(5)),
        };
        
        try
        {
            var slotsResponse = await GetInformationAboutSlots(searchRequest);
            slots = slotsResponse.ToList();
        }
        catch (Exception e)
        {
            var errMessage = "При получении информации о свободных слотах произошла ошибка";
            _logger.LogError(e, errMessage);
            
            answer = errMessage + ". Обратитесь к администратору бота.";
            await botClient.SendMessage(update.ChannelPost.Chat.Id, answer, cancellationToken: cancellationToken);
        }
        
        if (!slots.Any())
        {
            answer =
                $"Извините, свободных слотов в даты: {searchRequest.From.ToString()} по {searchRequest.Till.ToString()} - не найдено";
            await botClient.SendMessage(update.ChannelPost.Chat.Id, answer, cancellationToken: cancellationToken);
        }

        foreach (var slot in slots)
        {
            var message = GenerateAnswer(slot);
            
            if (_cache.TryGetValue(slot.Id, out var slotEntry))
            {
                if (slotEntry != null)
                {
                    continue;
                }
                else
                {
                     _cache.Set(slot.Id, message, TimeSpan.FromDays(5));
                }
            }
            
            await botClient.SendMessage(update.ChannelPost.Chat.Id, message, cancellationToken: cancellationToken);
            await Task.Delay(1000, cancellationToken);
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _logger.LogError(exception.Message, exception);
        return Task.FromException(exception);
    }

    private async Task<IEnumerable<Slot>> GetInformationAboutSlots(SearchRequest searchRequest)
    {
        var slotsResponse = await _activityService.FindSlots(searchRequest);
        return slotsResponse;
    }

    private string GenerateAnswer(Slot slot)
    {
        var sb = new StringBuilder();
        sb.AppendLine(slot.StartDateTime);
        sb.AppendLine(slot.Title);
        sb.AppendLine(slot.Location);
        sb.AppendLine(slot.Specialization);
        sb.AppendLine($"Продолжительность: {slot.Duration.TotalMinutes} мин");
        sb.AppendLine($"Осталось мест: {slot.Count}");
        sb.AppendLine();
        
        return sb.ToString();
    }
}