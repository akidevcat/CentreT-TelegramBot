using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CentreT_TelegramBot.Extensions;

public static class TelegramBotUpdateExtensions
{
    public static bool IsOfType(this Update update, UpdateType updateType) => 
        update.Type == updateType;

    public static bool IsCommand(this Update update, string command) =>
        IsOfType(update, UpdateType.Message) && update.Message!.Text?.ToLower().Split(" ").FirstOrDefault() == "/" + command.ToLower();
}