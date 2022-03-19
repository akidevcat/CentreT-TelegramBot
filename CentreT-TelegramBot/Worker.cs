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

    public Worker(ILogger<Worker> logger, IConfigurationService configuration, ITelegramService telegramService)
    {
        _logger = logger;
        _configuration = configuration;
        _telegramService = telegramService;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // Obtain bot token from configuration
        var botToken = _configuration.GetConfigurationObject<BotToken>();
        
        // If not defined - throw an exception
        if (botToken.Token == null)
            throw new ApplicationException(
                $"Configuration file {nameof(BotToken)}.json has no definition for {nameof(botToken.Token)}. Aborting.");
        
        // Create new telegram bot
        var bot = new TelegramBotClient(botToken.Token);
        ReceiverOptions receiverOptions = new() { };

        // Start polling with telegram service
        bot.StartReceiving(
            _telegramService.HandleUpdateAsync, 
            _telegramService.HandleErrorAsync, 
            receiverOptions, cancellationToken);
        
        // await bot.ReceiveAsync(
        //     _telegramService.HandleUpdateAsync,
        //     _telegramService.HandleErrorAsync,
        //     receiverOptions, cancellationToken);

        // Continue execution
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(1000, cancellationToken);
        }
    }
}