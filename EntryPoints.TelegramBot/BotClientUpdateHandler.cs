using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using UseCases.Handlers;

namespace EntryPoints.TelegramBot;

public class BotClientUpdateHandler : IUpdateHandler
{
    private readonly IMediator _mediator;
    private readonly ILogger<BotClientUpdateHandler> _logger;

    public BotClientUpdateHandler(IMediator mediator, ILogger<BotClientUpdateHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        if (update.ChannelPost != null)
        {
            await _mediator.Send(new ChannelClientUpdateRequest
            {
                ChannelPost = update.ChannelPost,
            }, cancellationToken);
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _logger.LogError(exception.Message, exception);
        return Task.FromException(exception);
    }
}