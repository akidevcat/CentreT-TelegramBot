using CentreT_TelegramBot.Attributes.Telegram.Bot;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace CentreT_TelegramBot.Services;

public class BotCoreService : IBotCoreService
{
    private readonly ILogger<BotCoreService> _logger;
    
    public BotCoreService(ILogger<BotCoreService> logger)
    {
        _logger = logger;
    }
    
    [ErrorHandler]
    protected Task OnBotError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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
    protected Task OnUserMessage(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        _logger.LogInformation(update.Id.ToString());
        
        return Task.CompletedTask;
    }
}