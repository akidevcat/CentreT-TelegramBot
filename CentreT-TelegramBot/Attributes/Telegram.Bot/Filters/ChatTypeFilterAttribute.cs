using Telegram.Bot.Types.Enums;

namespace CentreT_TelegramBot.Attributes.Telegram.Bot;

[AttributeUsage(AttributeTargets.Method)]
public class ChatTypeFilterAttribute : Attribute
{
    public ChatType ChatType { get; }

    public ChatTypeFilterAttribute(ChatType chatType)
    {
        ChatType = chatType;
    }
}