using EntryPoints.TelegramBot.BotCommands;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UseCases.Handlers;
using Utils.Settings;

namespace EntryPoints.TelegramBot;

public class BotClientUpdateHandler : IUpdateHandler
{
    private readonly IMediator _mediator;
    private readonly IBotCommandFactory _commandFactory;
    private readonly ILogger<BotClientUpdateHandler> _logger;
    private readonly IOptions<BotSettings> _botSettings;

    public BotClientUpdateHandler(IMediator mediator, IBotCommandFactory commandFactory, ILogger<BotClientUpdateHandler> logger, IOptions<BotSettings> botSettings)
    {
        _mediator = mediator;
        _commandFactory = commandFactory;
        _logger = logger;
        _botSettings = botSettings;
    }

    public async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message)
        {
            if (!IsMessageHandlingAvailable(update.Message.Chat.Username))
            {
                await botClient.SendMessage(update.Message.Chat.Id, "Бот приватный", cancellationToken: cancellationToken);
                
                _logger.LogWarning("Попытка воспользоваться букингом от пользователя: {username}. Сообщение: {text}",
                    update.Message.Chat.Username, update.Message.Text);
                return;
            }
            await HandleMessage(botClient, update.Message, cancellationToken);
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
    
    private async Task HandleMessage(ITelegramBotClient botClient,
        Message message,
        CancellationToken cancellationToken)
    {
        try
        {
            var commandType = BotCommandsHelper.GetCommandType(message.Text);
        
            var botCommand = _commandFactory.GetCommand(commandType);
            
            var response = await botCommand.Execute(message);

            await botClient.SendMessage(message.Chat.Id, response, cancellationToken: cancellationToken);
        }
        catch (ArgumentException e)
        {
            await botClient.SendMessage(message.Chat.Id, e.Message, cancellationToken: cancellationToken);
        }
    }

    private bool IsMessageHandlingAvailable(string username)
    {
        return _botSettings.Value.Admins.Split(',').Contains(username);
    }
}