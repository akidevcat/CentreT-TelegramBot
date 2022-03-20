namespace CentreT_TelegramBot.Attributes.Telegram.Bot;

[AttributeUsage(AttributeTargets.Method)]
public class FromBotFilterAttribute : Attribute
{
    public bool ShouldBeBot { get; }

    public FromBotFilterAttribute(bool shouldBeBot)
    {
        ShouldBeBot = shouldBeBot;
    }
}