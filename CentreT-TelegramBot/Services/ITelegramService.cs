using Telegram.Bot;
using Telegram.Bot.Types;

namespace CentreT_TelegramBot.Services;

public interface ITelegramService
{
    Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken);

    Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
}