using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoscowActivityServices.Abstractions;
using MoscowActivityServices.Abstractions.Models;
using NodaTime;
using Telegram.Bot;
using Utils;
using Utils.Settings;

namespace UseCases.Handlers;

public class SlotSearchingRequest : IRequest
{
    public LocalDate From { get; set; }
    public LocalDate Till { get; set; }
}

public class SlotSearchingHandler : IRequestHandler<SlotSearchingRequest>
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

    public async Task Handle(SlotSearchingRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        string answer = string.Empty;
        var slots = new List<Slot>();
        
        var searchRequest = new SearchRequest
        {
            From = request.From,
            Till = request.Till,
        };
        
        var channelId = _botSettings.Value.ChannelId;
        
        try
        {
            var slotsResponse = await _activityService.FindSlots(searchRequest);
            slots = slotsResponse.ToList();
        }
        catch (Exception e)
        {
            var errMessage = "При получении информации о свободных слотах произошла ошибка";
            _logger.LogError(e, errMessage);

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
}