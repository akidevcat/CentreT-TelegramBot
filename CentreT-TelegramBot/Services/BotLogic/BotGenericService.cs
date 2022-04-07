using CentreT_TelegramBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace CentreT_TelegramBot.Services;

public abstract class BotGenericService : IBotGenericService, IUpdateHandler, IErrorHandler
{
    public virtual Task RunAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    
    internal async Task ReplyUser(StateUpdate update, string message, 
        Task<object[]>? argumentsRetrieveTask = null, 
        string[]? buttons = null)
    {
        ReplyMarkupBase? markup = null;
        if (buttons != null)
        {
            if (buttons.Length == 0)
            {
                markup = new ReplyKeyboardRemove();
            }
            else
            {
                markup = new ReplyKeyboardMarkup(buttons.Select(x => new KeyboardButton(x)))
                {
                    ResizeKeyboard = true,
                    OneTimeKeyboard = false
                };
            }
        }

        var resultMessage = message;

        if (argumentsRetrieveTask != null)
        {
            var args = await argumentsRetrieveTask;
            resultMessage = string.Format(message, args);
        }

        await update.BotClient.SendTextMessageAsync(update.ChatId!, 
            resultMessage,
            replyToMessageId: update.BotUpdate.Message!.MessageId,
            cancellationToken: update.CancellationToken,
            replyMarkup: markup);
    }
}