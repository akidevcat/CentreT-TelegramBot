using CentreT_TelegramBot.Attributes.Telegram.Bot;
using CentreT_TelegramBot.Models;
using CentreT_TelegramBot.Models.States;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CentreT_TelegramBot.Services;

public class BotAdminService : BotStateMachineService<AdminState, AdminEvent>, IBotAdminService
{

    private readonly IRepositoryService _repositoryService;
    private readonly IConfigurationService _configurationService;
    private readonly ILogger<BotAdminService> _logger;
    
    public BotAdminService(IRepositoryService repositoryService, 
        ITelegramContext telegramContext, IConfigurationService configurationService, 
        ILogger<BotAdminService> logger)
    {
        _repositoryService = repositoryService;
        _configurationService = configurationService;
        _logger = logger;
    }
    
    public override Task RunAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    [IncludesCommandsFilter("/register")]
    [ChatTypeFilter(ChatType.Group)]
    protected async Task OnRegisterCommand(ITelegramBotClient c, Update u, CancellationToken t)
    {
        await EvaluateMachineAction(c, u, t, AdminEvent.RegisterCommand);
    }
    
    private async Task EvaluateMachineAction(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, 
        AdminEvent userEvent, object? argument = null)
    {
        var userId = update.Message!.From!.Id;
        var user = await _repositoryService.GetOrCreateUser(userId);
        var stateUpdate = new StateUpdate(botClient, update, cancellationToken);
        var machine = CreateStateMachine(user.State, userId.ToString());

        stateUpdate.Argument = argument;
        await machine.Fire(userEvent, stateUpdate);
    }
    
}