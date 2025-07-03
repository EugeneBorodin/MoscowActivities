using Microsoft.Extensions.Logging;
using MoscowActivityServices.Abstractions;
using MoscowActivityServices.Abstractions.Models;
using NodaTime;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Utils;

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

        if (update.ChannelPost == null)
        {
            return;
        }

        _logger.LogInformation(
            $"Update received: {update.ChannelPost.Text} in chat with id: {update.ChannelPost.Chat.Id}");

        if (update.ChannelPost.Text != "Покажи все слоты")
        {
            await botClient.SendMessage(update.ChannelPost.Chat.Id, "Чтобы узнать все слоты, нужно ввести: \"Покажи все слоты\"", cancellationToken: cancellationToken);
            return;
        }

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
            var message = BotExtensions.GenerateAnswer(slot);
            
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
}