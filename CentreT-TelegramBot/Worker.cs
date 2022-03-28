using CentreT_TelegramBot.Attributes;
using CentreT_TelegramBot.Attributes.Telegram.Bot;
using CentreT_TelegramBot.Models.Configuration;
using CentreT_TelegramBot.Repositories;
using CentreT_TelegramBot.Services;
using Microsoft.EntityFrameworkCore;
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
    private readonly IBotUserService _botUserService;
    private readonly BotDbContext _botDbContext;

    public Worker(ILogger<Worker> logger, IConfigurationService configuration, 
        ITelegramService telegramService, IBotUserService botUserService,
        BotDbContext dbContext)
    {
        _logger = logger;
        _configuration = configuration;
        _telegramService = telegramService;
        _botUserService = botUserService;
        _botDbContext = dbContext;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // Obtain bot token from configuration
        var botToken = _configuration.GetConfigurationObject<BotToken>();
        
        // If not defined - throw an exception
        if (botToken.Token == null)
            throw new ApplicationException(
                $"Configuration file {nameof(BotToken)}.json has no definition for {nameof(botToken.Token)}. Aborting.");

        // Check database connection
        if (!await _botDbContext.Database.CanConnectAsync(cancellationToken))
        {
            throw new ApplicationException(
                $"Cannot connect to database. Aborting.");
        }
        
        // Run telegram bot service
        _telegramService.Run(botToken.Token, cancellationToken);
        
        // Run bot logic services
        await _botUserService.RunAsync(cancellationToken);
    }
}