using CentreT_TelegramBot.Attributes.Telegram.Bot;
using CentreT_TelegramBot.Entities;
using CentreT_TelegramBot.Entities.States;
using CentreT_TelegramBot.Models.Configuration;
using CentreT_TelegramBot.Repositories;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CentreT_TelegramBot.Services;

public class BotCoreService : IBotCoreService
{
    private readonly IRepositoryService _repositoryService;
    private readonly ITelegramContext _telegramContext;
    private readonly IConfigurationService _configurationService;
    private readonly ILogger<BotCoreService> _logger;
    public BotCoreService(IRepositoryService repositoryService, 
        ITelegramContext telegramContext, IConfigurationService configurationService, 
        ILogger<BotCoreService> logger)
    {
        _repositoryService = repositoryService;
        _telegramContext = telegramContext;
        _configurationService = configurationService;
        _logger = logger;
    }
    
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        
    }
    
    [ErrorHandler]
    protected Task OnBotError(ITelegramBotClient telegramContext, Exception exception, CancellationToken cancellationToken)
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
    [CommandFilter("information")]
    [FromBotFilter(false)]
    [ChatTypeFilter(ChatType.Private)]
    protected async Task OnInformationCommand(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var botMessages = _configurationService.GetConfigurationObject<BotMessages>();
        var userId = update.Message!.From!.Id;
        var userContext = await _repositoryService.GetUserContext(userId);

        if (userContext.State != UserContextState.Start)
            return;

        await botClient.SendTextMessageAsync(update.Message.Chat.Id, botMessages.InformationMessage, cancellationToken: cancellationToken);
    }
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    [CommandFilter("join")]
    [FromBotFilter(false)]
    [ChatTypeFilter(ChatType.Private)]
    protected async Task OnJoinCommand(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var botMessages = _configurationService.GetConfigurationObject<BotMessages>();
        var userId = update.Message!.From!.Id;
        var userContext = await _repositoryService.GetUserContext(userId);

        if (userContext.State != UserContextState.Start)
            return;

        var userJoinContext = await _repositoryService.GetUserJoinContext(userId);
        
        userContext.State = UserContextState.JoinRequest;
        await _repositoryService.SaveChanges();
        //await _repositoryService.UpdateUserContext(userContext);

        await botClient.SendTextMessageAsync(update.Message.Chat.Id, botMessages.JoinMessage, cancellationToken: cancellationToken);
    }
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    [CommandFilter("back")]
    [FromBotFilter(false)]
    [ChatTypeFilter(ChatType.Private)]
    protected async Task OnBackCommand(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var userId = update.Message!.From!.Id;
        var userContext = await _repositoryService.GetUserContext(userId);
    }
}