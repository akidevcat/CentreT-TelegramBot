using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CentreT_TelegramBot.Extensions;

public static class TelegramBotUpdateExtensions
{
    public static bool IsOfType(this Update update, UpdateType updateType) => update.Type == updateType;
}