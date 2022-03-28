namespace CentreT_TelegramBot.Services;

public abstract class BotGenericService : IBotGenericService, IUpdateHandler, IErrorHandler
{
    public virtual Task RunAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    
}