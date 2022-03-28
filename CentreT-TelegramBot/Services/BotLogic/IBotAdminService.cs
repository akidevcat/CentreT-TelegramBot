namespace CentreT_TelegramBot.Services;

public interface IBotAdminService
{
    Task RunAsync(CancellationToken cancellationToken);
}