using Telegram.Bot;

namespace CentreT_TelegramBot.Services;

public interface ITelegramContext
{
    public TelegramBotClient? BotClient { get; set; }
}