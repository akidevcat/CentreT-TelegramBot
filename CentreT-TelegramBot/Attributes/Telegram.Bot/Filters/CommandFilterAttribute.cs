namespace CentreT_TelegramBot.Attributes.Telegram.Bot;

[AttributeUsage(AttributeTargets.Method)]
public class CommandFilterAttribute : Attribute
{
    public string Command { get; }

    public CommandFilterAttribute(string command)
    {
        Command = command;
    }
}