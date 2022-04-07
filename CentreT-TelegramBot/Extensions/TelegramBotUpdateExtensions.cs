using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CentreT_TelegramBot.Extensions;

public static class TelegramBotUpdateExtensions
{
    public static bool IsOfType(this Update update, UpdateType updateType) => 
        update.Type == updateType;

    // public static bool StartsWithCommand(this Update update, string command)
    // {
    //     if (!IsOfType(update, UpdateType.Message))
    //     {
    //         return false;
    //     }
    //
    //     var text = update.Message?.Text?.ToLower();
    //     var firstArg = text?.Split(" ").FirstOrDefault();
    //
    //     // If command pattern is empty
    //     if (command.Length == 0)
    //     {
    //         return firstArg != null;
    //     }
    //
    //     return firstArg == command.ToLower();
    // }
        
    public static bool StartsWithCommands(this Update update, ISet<string> commands)
    {
        if (!IsOfType(update, UpdateType.Message))
        {
            return false;
        }

        var text = update.Message?.Text?.ToLower();
        var firstArg = text?.Split(" ").FirstOrDefault();

        if (firstArg == null)
            return false;
        
        return commands.Contains(firstArg);
    }
    
    public static bool StartsWithBackslash(this Update update)
    {
        if (!IsOfType(update, UpdateType.Message))
        {
            return false;
        }

        var text = update.Message?.Text?.ToLower();
        var firstArg = text?.Split(" ").FirstOrDefault();

        if (firstArg == null)
            return false;
        
        return firstArg.StartsWith("/");
    }

    public static bool HasSeveralArguments(this Update update, int argumentCount)
    {
        if (!IsOfType(update, UpdateType.Message))
        {
            return false;
        }

        var textArgumentCount = update.Message?.Text?.Split(" ").Length;
        return argumentCount + 1 == textArgumentCount;
    }

    public static bool IsMessageFromNull(this Update update) =>
        IsOfType(update, UpdateType.Message) && update.Message!.From == null;
    
    public static bool IsMessageFromBot(this Update update) =>
        !IsMessageFromNull(update) && update.Message!.From!.IsBot;

    public static bool IsChatOfType(this Update update, ChatType chatType) =>
        IsOfType(update, UpdateType.Message) && update.Message!.Chat.Type == chatType;
}