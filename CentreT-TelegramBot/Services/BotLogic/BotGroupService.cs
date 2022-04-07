using CentreT_TelegramBot.Attributes.Telegram.Bot;
using CentreT_TelegramBot.Models;
using CentreT_TelegramBot.Models.Configuration;
using CentreT_TelegramBot.Models.States;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CentreT_TelegramBot.Services;

public class BotGroupService : BotGenericService, IBotGroupService
{
    private readonly IRepositoryService _repositoryService;
    private readonly IConfigurationService _configurationService;
    private readonly ILogger<BotGroupService> _logger;
    
    public BotGroupService(IRepositoryService repositoryService, 
        IConfigurationService configurationService, 
        ILogger<BotGroupService> logger)
    {
        _repositoryService = repositoryService;
        _configurationService = configurationService;
        _logger = logger;
    }
    
    public override Task RunAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    
    [ErrorHandler]
    protected Task OnBotException(ITelegramBotClient telegramContext, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };
        
        _logger.LogError("{ErrorMessage}", errorMessage);
        
        return Task.CompletedTask;
    }
    
    #region Commands
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    [IncludesCommandsFilter("/register")]
    [HasArgumentsFilter(1)]
    [ChatTypeFilter(ChatType.Group)]
    protected async Task OnRegisterCommand(ITelegramBotClient c, Update u, CancellationToken t)
    {
        var m = _configurationService?.GetConfigurationObject<BotMessages>()!;
        var arg = u.Message!.Text!.Split(" ")[1];

        var user = await _repositoryService.GetOrCreateUser(u.Message.From!.Id);
        if (user.Status != UserStatus.Admin)
        {
            await ReplyUser(new StateUpdate(c, u, t), m.NotEnoughPermissions);
            return;
        }
        
        var created = await RegisterChat(u.Message!.Chat.Id, arg);

        if (created)
        {
            await ReplyUser(new StateUpdate(c, u, t), m.ChatRegistrationCompleted);
        }
        else
        {
            await ReplyUser(new StateUpdate(c, u, t), m.ChatRegistrationFailed);
        }
    }
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    [IncludesCommandsFilter("/unregister")]
    [ChatTypeFilter(ChatType.Group)]
    protected async Task OnUnregisterCommand(ITelegramBotClient c, Update u, CancellationToken t)
    {
        var m = _configurationService?.GetConfigurationObject<BotMessages>()!;
        
        var user = await _repositoryService.GetOrCreateUser(u.Message!.From!.Id);
        if (user.Status != UserStatus.Admin)
        {
            await ReplyUser(new StateUpdate(c, u, t), m.NotEnoughPermissions);
            return;
        }
        
        if (await UnregisterChat(u.Message!.Chat.Id))
        {
            await ReplyUser(new StateUpdate(c, u, t), m.ChatUnregistrationCompleted);
        }
        else
        {
            await ReplyUser(new StateUpdate(c, u, t), m.ChatUnregistrationFailed);
        }
    }
    
    #endregion
    
    private async Task<bool> RegisterChat(long id, string chatName)
    {
        return await _repositoryService.CreateChat(new Models.Chat(id, chatName)) != null;
    }
    
    private async Task<bool> UnregisterChat(long id)
    {
        return await _repositoryService.DeleteChat(id);
    }
}