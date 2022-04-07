namespace CentreT_TelegramBot.Services;

public interface IBotGroupService
{
    Task RunAsync(CancellationToken cancellationToken);
}