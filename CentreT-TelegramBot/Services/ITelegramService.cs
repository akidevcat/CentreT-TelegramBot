using Telegram.Bot;
using Telegram.Bot.Types;

namespace CentreT_TelegramBot.Services;

public interface ITelegramService
{
    Task RunAsync(string botToken, CancellationToken cancellationToken);
}