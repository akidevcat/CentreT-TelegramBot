using Telegram.Bot;
using Telegram.Bot.Types;

namespace CentreT_TelegramBot.Services;

public interface ITelegramService
{
    void Run(string botToken, CancellationToken cancellationToken);
}