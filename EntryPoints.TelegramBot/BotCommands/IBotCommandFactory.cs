namespace EntryPoints.TelegramBot.BotCommands;

public interface IBotCommandFactory
{
    public IBotCommand GetCommand(string message);
}