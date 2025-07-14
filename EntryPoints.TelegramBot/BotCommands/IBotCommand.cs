using Telegram.Bot.Types;

namespace EntryPoints.TelegramBot.BotCommands;

public interface IBotCommand
{
    public Task<string> Execute(Message message);
}