using MediatR;
using Microsoft.Extensions.Logging;
using MoscowActivityServices.Abstractions;
using MoscowActivityServices.Abstractions.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace UseCases.Handlers;

public class MessageClientUpdateRequest : IRequest
{
    public Message Message { get; set; }
}

public class MessageClientUpdateHandler: IRequestHandler<MessageClientUpdateRequest>
{
    private readonly IActivityService _activityService;
    private readonly IBookingConfigService _bookingConfigService;
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<ChannelClientUpdateHandler> _logger;
    
    public MessageClientUpdateHandler(IActivityService activityService, IBookingConfigService bookingConfigService, ITelegramBotClient botClient, ILogger<ChannelClientUpdateHandler> logger)
    {
        _activityService = activityService;
        _bookingConfigService = bookingConfigService;
        _botClient = botClient;
        _logger = logger;
    }
    
    public async Task Handle(MessageClientUpdateRequest request, CancellationToken cancellationToken)
    {
        var bookingConfig = await _bookingConfigService.GetBookingConfig();
        var userConfig = bookingConfig.UserConfigs.FirstOrDefault(c => c.Username == request.Message.Chat.Username);

        if (userConfig != null)
        {
            await _botClient.SendMessage(request.Message.Chat.Id, $"{userConfig.Username} existed", cancellationToken: cancellationToken);
            
            return;
        }
        
        userConfig = new UserConfig
        {
            Username = request.Message.Chat.Username,
        };
            
        bookingConfig.UserConfigs.Add(userConfig);
        
        await _bookingConfigService.UpdateBookingConfig(bookingConfig);
        
        await _botClient.SendMessage(request.Message.Chat.Id, $"{userConfig.Username} updated", cancellationToken: cancellationToken);
    }
}