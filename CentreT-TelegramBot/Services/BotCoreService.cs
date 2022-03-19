using CentreT_TelegramBot.Attributes.Telegram.Bot;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CentreT_TelegramBot.Services;

public class BotCoreService : IBotCoreService
{
    private readonly HashSet<long> _chats;

    private readonly ITelegramContext _telegramContext;
    private readonly ILogger<BotCoreService> _logger;

    public BotCoreService(ITelegramContext telegramContext, ILogger<BotCoreService> logger)
    {
        _telegramContext = telegramContext;
        _logger = logger;
        
        _chats = new HashSet<long>();
    }
    
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            
            await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
        }
    }
    
    [ErrorHandler]
    protected Task OnBotError(ITelegramContext telegramContext, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };
        
        _logger.LogError(errorMessage);
        
        return Task.CompletedTask;
    }
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    [CommandFilter("register")]
    protected async Task OnUserMessage(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var chatId = update.Message!.Chat.Id;

        if (!_chats.Contains(chatId))
        {
            _chats.Add(chatId);
            await botClient.SendTextMessageAsync(chatId, "Successfully registered!", cancellationToken: cancellationToken);
        }
        else
        {
            await botClient.SendTextMessageAsync(chatId, "You are already registered.", cancellationToken: cancellationToken);
        }
    }
}