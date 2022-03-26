namespace CentreT_TelegramBot.Attributes.Telegram.Bot;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ExcludesCommandsFilterAttribute : Attribute
{
    public ISet<string> Commands { get; }

    public ExcludesCommandsFilterAttribute(params string[] commands)
    {
        Commands = new HashSet<string>(commands);
    }
}