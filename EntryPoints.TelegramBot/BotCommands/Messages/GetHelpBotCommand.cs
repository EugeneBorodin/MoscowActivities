using System.Text;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace EntryPoints.TelegramBot.BotCommands.Messages;

public class GetHelpBotCommand : IBotCommand
{
    private readonly IMediator _mediator;
    private readonly ILogger<GetHelpBotCommand> _logger;

    public GetHelpBotCommand(IMediator mediator, ILogger<GetHelpBotCommand> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public Task<string> Execute(Message message)
    {
        return Task.FromResult(GenerateHelpMessage(message.Chat.Username));
    }

    private string GenerateHelpMessage(string username)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Привет, {username}!");
        sb.AppendLine();
        sb.AppendLine("Чтобы задать конфигурацию для автозаписи воспользуйся командой");
        sb.AppendLine();
        sb.AppendLine("/задать конфигурацию");
        sb.AppendLine("Имя: Татьяна");
        sb.AppendLine($"Телефон: 7XXXXXXXXXX");
        sb.AppendLine($"Email: your@email.com");
        sb.AppendLine();
        sb.AppendLine("Слоты:");
        sb.AppendLine("Понедельник, 19:00, 4м");
        sb.AppendLine("Понедельник, 19:30, 4м");
        sb.AppendLine("Среда, 12:00, 4м");
        sb.AppendLine("Среда, 12:30, 4м");
        sb.AppendLine("Среда, 13:00, 4м");
        sb.AppendLine("Среда, 13:30, 4м");
        sb.AppendLine();
        sb.AppendLine("Чтобы посмотреть конфигурацию воспользуйся командой");
        sb.AppendLine();
        sb.AppendLine("/посмотреть конфигурацию");
        sb.AppendLine();
        sb.AppendLine("Если забыл остальные команды - воспользуйся командой");
        sb.AppendLine();
        sb.AppendLine("/справка");
        sb.AppendLine();
        sb.AppendLine("Всем PADEL!");

        return sb.ToString();
    }
}