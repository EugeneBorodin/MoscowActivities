using EntryPoints.TelegramBot.BotCommands;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UseCases.Handlers;

namespace EntryPoints.TelegramBot;

public class BotClientUpdateHandler : IUpdateHandler
{
    private readonly IMediator _mediator;
    private readonly IBotCommandFactory _commandFactory;
    private readonly ILogger<BotClientUpdateHandler> _logger;

    public BotClientUpdateHandler(IMediator mediator, IBotCommandFactory commandFactory, ILogger<BotClientUpdateHandler> logger)
    {
        _mediator = mediator;
        _commandFactory = commandFactory;
        _logger = logger;
    }

    public async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message)
        {
            var commandType = BotCommandsHelper.GetCommandType(update.Message.Text);
        
            var botCommand = _commandFactory.GetCommand(commandType);
            
            var response = await botCommand.Execute(update.Message);

            await botClient.SendMessage(update.Message.Chat.Id, response, cancellationToken: cancellationToken);
        }
        
        else if (update.Type == UpdateType.ChannelPost)
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