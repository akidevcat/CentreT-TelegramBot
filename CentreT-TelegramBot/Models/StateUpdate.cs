using Telegram.Bot;
using Telegram.Bot.Types;

namespace CentreT_TelegramBot.Models;

internal class StateUpdate
{
    public ITelegramBotClient BotClient { get; set; }
    public Update BotUpdate { get; set; }
    public CancellationToken CancellationToken { get; set; }
    public object? Argument;
    public string? ArgumentErrorMessage;

    public long? UserId => BotUpdate?.Message?.From?.Id;
    public long? ChatId => BotUpdate?.Message?.Chat.Id;
        
    public StateUpdate(ITelegramBotClient botClient, Update botUpdate, CancellationToken cancellationToken, object? argument = null)
    {
        BotClient = botClient;
        BotUpdate = botUpdate;
        CancellationToken = cancellationToken;
        Argument = argument;
        ArgumentErrorMessage = null;
    }

    public void Deconstruct(out ITelegramBotClient botClient, 
        out long? chatId, out long? userId, 
        out CancellationToken cancellationToken)
    {
        botClient = BotClient;
        chatId = UserId;
        userId = UserId;
        cancellationToken = CancellationToken;
    }
}