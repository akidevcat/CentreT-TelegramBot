namespace CentreT_TelegramBot.Services;

public class BotAdminService : IBotAdminService
{

    
    
    public BotAdminService()
    {
        
    }
    
    public Task RunAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    
}