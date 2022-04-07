namespace CentreT_TelegramBot.Attributes.Telegram.Bot;

[AttributeUsage(AttributeTargets.Method)]
public class HasArgumentsFilterAttribute : Attribute
{
    public int ArgumentCount { get; }
    
    public HasArgumentsFilterAttribute(int argumentCount)
    {
        ArgumentCount = argumentCount;
    }
}