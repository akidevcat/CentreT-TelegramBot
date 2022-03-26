namespace CentreT_TelegramBot.Attributes.Telegram.Bot;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class IncludesCommandsFilterAttribute : Attribute
{
    public ISet<string> Commands { get; }

    public IncludesCommandsFilterAttribute(params string[] commands)
    {
        Commands = new HashSet<string>(commands);
    }
}