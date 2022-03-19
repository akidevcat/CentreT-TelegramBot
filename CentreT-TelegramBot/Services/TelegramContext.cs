using Telegram.Bot;

namespace CentreT_TelegramBot.Services;

public class TelegramContext : ITelegramContext
{
    public TelegramBotClient? BotClient { get; set; }
}