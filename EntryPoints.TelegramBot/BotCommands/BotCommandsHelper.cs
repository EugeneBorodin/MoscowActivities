namespace EntryPoints.TelegramBot.BotCommands;

public static class BotCommandsHelper
{
    public static BotCommandType GetCommandType(string text)
    {
        var data = text.Trim();
        
        if (data.StartsWith("/задать конфигурацию"))
        {
            return BotCommandType.UpdateUserConfig;
        }
        
        if (data.StartsWith("/посмотреть конфигурацию"))
        {
            return BotCommandType.GetUserConfig;
        }
        
        if (data.StartsWith("/справка"))
        {
            return BotCommandType.GetHelp;
        }

        return BotCommandType.Unknown;
    }
}