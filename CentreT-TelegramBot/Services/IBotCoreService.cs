using CentreT_TelegramBot.Attributes.Telegram.Bot;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CentreT_TelegramBot.Services;

public interface IBotCoreService : IUpdateHandler, IErrorHandler
{
    Task RunAsync(CancellationToken cancellationToken);
}