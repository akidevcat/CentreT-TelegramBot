namespace CentreT_TelegramBot.Services;

public interface IBotGenericService
{
    Task RunAsync(CancellationToken cancellationToken);
}