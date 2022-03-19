using Telegram.Bot.Types.Enums;

namespace CentreT_TelegramBot.Attributes.Telegram.Bot;

[AttributeUsage(AttributeTargets.Method)]
public class UpdateTypeFilterAttribute : Attribute
{
    public UpdateType UpdateType { get; }

    public UpdateTypeFilterAttribute(UpdateType updateType)
    {
        UpdateType = updateType;
    }
}