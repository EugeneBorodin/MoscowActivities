using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MoscowActivityServices.Abstractions;
using MoscowActivityServices.Abstractions.Models;
using NodaTime;
using Telegram.Bot;
using Utils;
using Utils.Settings;

namespace Web.Handlers;

public class SlotSearchingHandler
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<SlotSearchingHandler> _logger;
    private readonly IActivityService _activityService;
    private readonly IOptions<BotSettings> _botSettings;
    private readonly ITelegramBotClient _botClient;

    public SlotSearchingHandler(
        IMemoryCache cache,
        ILogger<SlotSearchingHandler> logger,
        IActivityService activityService,
        IOptions<BotSettings> botSettings,
        ITelegramBotClient botClient)
    {
        _cache = cache;
        _logger = logger;
        _activityService = activityService;
        _botSettings = botSettings;
        _botClient = botClient;
    }

    public async Task Handle(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        string answer = string.Empty;
        var slots = new List<Slot>();
        
        var today = LocalDate.FromDateTime(DateTime.Now);
        var searchRequest = new SearchRequest
        {
            From = today,
            Till = LocalDate.Add(today, Period.FromDays(5)),
        };
        
        var channelId = _botSettings.Value.ChannelId;
        
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
            await _botClient.SendMessage(channelId, answer, cancellationToken: cancellationToken);

            throw;
        }

        foreach (var slot in slots)
        {
            var message = BotExtensions.GenerateAnswer(slot);
            
            if (_cache.TryGetValue(slot.Id, out _))
            {
                continue;
            }
            
            _cache.Set(slot.Id, message, TimeSpan.FromDays(5));
            
            await _botClient.SendMessage(channelId, message, cancellationToken: cancellationToken);
            await Task.Delay(500, cancellationToken);
        }
    }
    
    private async Task<IEnumerable<Slot>> GetInformationAboutSlots(SearchRequest searchRequest)
    {
        var slotsResponse = await _activityService.FindSlots(searchRequest);
        return slotsResponse;
    }
}