using CentreT_TelegramBot.Attributes;
using CentreT_TelegramBot.Attributes.Telegram.Bot;
using CentreT_TelegramBot.Models.Configuration;
using CentreT_TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace CentreT_TelegramBot;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfigurationService _configuration;
    private readonly ITelegramService _telegramService;
    private readonly IBotCoreService _botCoreService;

    public Worker(ILogger<Worker> logger, IConfigurationService configuration, ITelegramService telegramService, IBotCoreService botCoreService)
    {
        _logger = logger;
        _configuration = configuration;
        _telegramService = telegramService;
        _botCoreService = botCoreService;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // Obtain bot token from configuration
        var botToken = _configuration.GetConfigurationObject<BotToken>();
        
        // If not defined - throw an exception
        if (botToken.Token == null)
            throw new ApplicationException(
                $"Configuration file {nameof(BotToken)}.json has no definition for {nameof(botToken.Token)}. Aborting.");
        
        // Run telegram bot service
        _telegramService.RunAsync(botToken.Token, cancellationToken).Start();
        
        // Run bot logic service
        _botCoreService.RunAsync(cancellationToken).Start();
    }
}