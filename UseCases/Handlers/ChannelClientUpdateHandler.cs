using MediatR;
using Microsoft.Extensions.Logging;
using MoscowActivityServices.Abstractions;
using MoscowActivityServices.Abstractions.Models;
using NodaTime;
using Telegram.Bot;
using Telegram.Bot.Types;
using Utils;

namespace UseCases.Handlers;

public class ChannelClientUpdateRequest : IRequest
{
    public Message ChannelPost { get; set; }
}

public class ChannelClientUpdateHandler: IRequestHandler<ChannelClientUpdateRequest>
{
    private readonly IActivityService _activityService;
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<ChannelClientUpdateHandler> _logger;

    public ChannelClientUpdateHandler(IActivityService activityService, ITelegramBotClient botClient, ILogger<ChannelClientUpdateHandler> logger)
    {
        _activityService = activityService;
        _botClient = botClient;
        _logger = logger;
    }
    
    public async Task Handle(ChannelClientUpdateRequest request, CancellationToken cancellationToken)
    { 
        cancellationToken.ThrowIfCancellationRequested();

        _logger.LogInformation(
            $"Update received: {request.ChannelPost.Text} in chat with id: {request.ChannelPost.Chat.Id}");

        if (request.ChannelPost.Text != "Покажи все слоты")
        {
            await _botClient.SendMessage(request.ChannelPost.Chat.Id, "Чтобы узнать все слоты, нужно ввести: \"Покажи все слоты\"", cancellationToken: cancellationToken);
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
            var slotsResponse = await _activityService.FindSlots(searchRequest);
            slots = slotsResponse.ToList();
        }
        catch (Exception e)
        {
            var errMessage = "При получении информации о свободных слотах произошла ошибка";
            _logger.LogError(e, errMessage);
            
            answer = errMessage + ". Обратитесь к администратору бота.";
            await _botClient.SendMessage(request.ChannelPost.Chat.Id, answer, cancellationToken: cancellationToken);
        }
        
        if (!slots.Any())
        {
            answer =
                $"Извините, свободных слотов в даты: {searchRequest.From.ToString()} по {searchRequest.Till.ToString()} - не найдено";
            await _botClient.SendMessage(request.ChannelPost.Chat.Id, answer, cancellationToken: cancellationToken);
        }

        foreach (var slot in slots)
        {
            var message = BotExtensions.GenerateAnswer(slot);
            
            await _botClient.SendMessage(request.ChannelPost.Chat.Id, message, cancellationToken: cancellationToken);
            await Task.Delay(1000, cancellationToken);
        }
    }
}