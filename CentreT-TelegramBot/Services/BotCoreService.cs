using Appccelerate.StateMachine;
using Appccelerate.StateMachine.AsyncMachine;
using Appccelerate.StateMachine.AsyncMachine.Events;
using Appccelerate.StateMachine.AsyncMachine.Reports;
using CentreT_TelegramBot.Attributes.Telegram.Bot;
using CentreT_TelegramBot.Entities;
using CentreT_TelegramBot.Entities.States;
using CentreT_TelegramBot.Extensions;
using CentreT_TelegramBot.Models.Configuration;
using CentreT_TelegramBot.Repositories;
using CentreT_TelegramBot.StateMachine;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using User = CentreT_TelegramBot.Entities.User;

namespace CentreT_TelegramBot.Services;

public class BotCoreService : IBotCoreService
{

    internal class StateUpdate
    {
        public ITelegramBotClient BotClient { get; set; }
        public Update BotUpdate { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public object? Argument;
        public string? ArgumentErrorMessage;

        public long? UserId => BotUpdate?.Message?.From?.Id;
        public long? ChatId => BotUpdate?.Message?.Chat.Id;
        
        public StateUpdate(ITelegramBotClient botClient, Update botUpdate, CancellationToken cancellationToken, object? argument = null)
        {
            BotClient = botClient;
            BotUpdate = botUpdate;
            CancellationToken = cancellationToken;
            Argument = argument;
            ArgumentErrorMessage = null;
        }

        public void Deconstruct(out ITelegramBotClient botClient, 
            out long? chatId, out long? userId, 
            out CancellationToken cancellationToken)
        {
            botClient = BotClient;
            chatId = UserId;
            userId = UserId;
            cancellationToken = CancellationToken;
        }
    }

    private readonly StateMachineDefinition<UserState, UserEvent> _stateMachineDefinition;

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

        _stateMachineDefinition = CreateStateMachineDefinition();
    }
    
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        
    }
    
    [ErrorHandler]
    protected Task OnBotException(ITelegramBotClient telegramContext, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };
        
        _logger.LogError(errorMessage);
        
        return Task.CompletedTask;
    }

    private void OnTransitionException(object? sender, TransitionExceptionEventArgs<UserState, UserEvent> args)
    {
        _logger.LogError("Exception thrown: {Exception}", args.Exception.ToString());
    }
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    [CommandFilter("")] // Not a command
    [ChatTypeFilter(ChatType.Private)]
    protected async Task OnMessage(ITelegramBotClient c, Update u, CancellationToken t)
    {
        if (u.Message == null)
            return;
        
        await EvaluateMachineAction(c, u, t, UserEvent.ArgumentFilled, u.Message.Text);
    }
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    [CommandFilter("start")]
    [ChatTypeFilter(ChatType.Private)]
    protected async Task OnStartCommand(ITelegramBotClient c, Update u, CancellationToken t)
    {
        await EvaluateMachineAction(c, u, t, UserEvent.StartCommand);
    }
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    [CommandFilter("information")]
    [ChatTypeFilter(ChatType.Private)]
    protected async Task OnInformationCommand(ITelegramBotClient c, Update u, CancellationToken t)
    {
        await EvaluateMachineAction(c, u, t, UserEvent.InformationCommand);
    }
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    [CommandFilter("profile")]
    [ChatTypeFilter(ChatType.Private)]
    protected async Task OnProfileCommand(ITelegramBotClient c, Update u, CancellationToken t)
    {
        await EvaluateMachineAction(c, u, t, UserEvent.ProfileCommand);
    }
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    [CommandFilter("join")]
    [ChatTypeFilter(ChatType.Private)]
    protected async Task OnJoinCommand(ITelegramBotClient c, Update u, CancellationToken t)
    {
        await EvaluateMachineAction(c, u, t, UserEvent.JoinCommand);
    }
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    [CommandFilter("back")]
    [ChatTypeFilter(ChatType.Private)]
    protected async Task OnBackCommand(ITelegramBotClient c, Update u, CancellationToken t)
    {
        await EvaluateMachineAction(c, u, t, UserEvent.BackCommand);
    }
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    [CommandFilter("confirm")]
    [ChatTypeFilter(ChatType.Private)]
    protected async Task OnConfirmCommand(ITelegramBotClient c, Update u, CancellationToken t)
    {
        await EvaluateMachineAction(c, u, t, UserEvent.ConfirmCommand);
    }
    
    private async Task EvaluateMachineAction(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, 
        UserEvent userEvent, object? argument = null)
    {
        var userId = update.Message!.From!.Id;
        var user = await _repositoryService.GetOrCreateUser(userId);
        var stateUpdate = new StateUpdate(botClient, update, cancellationToken);
        var machine = CreateStateMachine(user.State, userId.ToString());

        stateUpdate.Argument = argument;
        await machine.Fire(userEvent, stateUpdate);
    }

    internal AsyncPassiveStateMachine<UserState, UserEvent> CreateStateMachine(UserState state, string name = nameof(BotCoreService))
    {
        var machine = _stateMachineDefinition.CreatePassiveStateMachine(name);
        
        machine.TransitionExceptionThrown += OnTransitionException;
        machine.TransitionDeclined += OnTransitionDeclined;

        machine.Load(new SimpleAsyncStateLoader(state));
        machine.Start();

        return machine;
    }

    internal StateMachineDefinition<UserState, UserEvent> CreateStateMachineDefinition()
    {
        var m = _configurationService.GetConfigurationObject<BotMessages>();
        var builder = new StateMachineDefinitionBuilder<UserState, UserEvent>();

        #region Entry
        
        // Entry -> ProfileGetName
        builder.In(UserState.Entry)
            .On(UserEvent.StartCommand)
            .Goto(UserState.ProfileGetName)
            .Execute<StateUpdate>(u => ReplyUser(u, m.EntryMessage));

        #endregion

        #region Start
        
        // -> Start
        builder.In(UserState.Start)
            .ExecuteOnEntry<StateUpdate>(u => ReplyUserWithButtons(u, m.StartMessage, "/information"));
        // Start -> Start [/info]
        builder.In(UserState.Start)
            .On(UserEvent.InformationCommand)
            .Execute<StateUpdate>(u => ReplyUser(u, m.InformationMessage));
        // Start -> Profile [/profile]
        builder.In(UserState.Start)
            .On(UserEvent.ProfileCommand)
            .Goto(UserState.Profile);
        // Start -> JoinConfirmation [/join]
        builder.In(UserState.Start)
            .On(UserEvent.JoinCommand)
            .If<StateUpdate>(u => UserJoinRequestsIsInLimit(u, 5))
            .Goto(UserState.JoinConfirmation)
            .Execute<StateUpdate>(CreateUserJoinRequest)
            .Execute<StateUpdate>(u => ReplyUser(u, m.JoinConfirmationMessage));
        
        #endregion

        #region ProfileGetName
        
        // -> ProfileGetName
        builder.In(UserState.ProfileGetName)
            .ExecuteOnEntry<StateUpdate>(u => ReplyUser(u, m.ProfileGetNameMessage));
        // ProfileGetName -> Profile
        builder.In(UserState.ProfileGetName)
            .On(UserEvent.BackCommand)
            .If<StateUpdate>(UserIsCompleted)
            .Goto(UserState.Profile);
        // ProfileGetName -> ProfileGetPronouns [/next]
        builder.In(UserState.ProfileGetName)
            .On(UserEvent.NextCommand)
            .If<StateUpdate>(u => UserPropertyNotNull(u, UserProperty.Name))
                .Goto(UserState.ProfileGetPronouns)
            .Otherwise()
                .Execute<StateUpdate>(u => ReplyUser(u, u.ArgumentErrorMessage!));
        // ProfileGetName -> ProfileGetPronouns
        builder.In(UserState.ProfileGetName)
            .On(UserEvent.ArgumentFilled)
            .If<StateUpdate>(UserNameIsValid)
                .Goto(UserState.ProfileGetPronouns)
                .Execute<StateUpdate>(u => SaveUserPropertyFromArgument(u, UserProperty.Name))
            .Otherwise()
                .Execute<StateUpdate>(u => ReplyUser(u, u.ArgumentErrorMessage!));
        
        #endregion
        
        // Add state sync
        builder.AddStateSync<UserState, UserEvent, StateUpdate>(SyncUserState);

        builder.WithInitialState(UserState.Entry);

        return builder.Build();
    }
    
    #region StateMachineActions

    private void OnTransitionDeclined(object? sender, TransitionEventArgs<UserState, UserEvent> e)
    {
        var botMessages = _configurationService.GetConfigurationObject<BotMessages>();
        var update = (StateUpdate)e.EventArgument;
        // Exception is handled by the bot API
        update.BotClient.SendTextMessageAsync(update.ChatId!, 
            botMessages.InvalidAction,
            cancellationToken: update.CancellationToken);
    }

    private async Task ReplyUser(StateUpdate update, string message)
    {
        // var user = update.BotUpdate.Message?.From;
        //
        // var stringReplacements = new Dictionary<string, string>()
        // {
        //     { "{userId}", user?.Id.ToString() ?? "null" },
        //     { "{}", user?. ?? "null" },
        // };
        
        await update.BotClient.SendTextMessageAsync(update.ChatId!, 
            message,
            cancellationToken: update.CancellationToken);
    }
    
    private async Task ReplyUserWithButtons(StateUpdate update, string message, params string[] buttons)
    {
        var markup = new ReplyKeyboardMarkup(buttons.Select(x => new KeyboardButton(x)));
        
        await update.BotClient.SendTextMessageAsync(update.ChatId!, 
            message,
            replyMarkup: markup,
            cancellationToken: update.CancellationToken);
    }

    private async Task SaveUserPropertyFromArgument(StateUpdate update, UserProperty property)
    {
        var (_, _, userId, token) = update;
        
        var user = await _repositoryService.GetOrCreateUser(userId!.Value);
        var arg = (string) update.Argument!;

        switch (property)
        {
            case UserProperty.Name:
                user.Name = arg;
                break;
            case UserProperty.Pronouns:
                user.Pronouns = arg;
                break;
            case UserProperty.Age:
                user.Age = short.Parse(arg);
                break;
            case UserProperty.Location:
                user.Location = arg;
                break;
        }

        await _repositoryService.SaveChanges();
    }
    
    private async Task ProfileGetNameEntryAction(StateUpdate update)
    {
        var botMessages = _configurationService.GetConfigurationObject<BotMessages>();
        var (botClient, chatId, _, token) = update;
        
        await botClient.SendTextMessageAsync(chatId!, 
            botMessages.ProfileGetNameMessage,
            cancellationToken: token);
    }

    private async Task<bool> UserPropertyNotNull(StateUpdate update, UserProperty property)
    {
        var botMessages = _configurationService.GetConfigurationObject<BotMessages>();
        var (_, _, userId, token) = update;
        
        var user = await _repositoryService.GetOrCreateUser(userId!.Value);

        update.ArgumentErrorMessage = botMessages.ShouldNotBeEmpty;
        
        return property switch
        {
            UserProperty.Name => user.Name != null,
            UserProperty.Pronouns => user.Pronouns != null,
            UserProperty.Age => user.Age != null,
            UserProperty.Location => user.Location != null,
            _ => false
        };
    }

    private async Task CreateUserJoinRequest(StateUpdate update)
    {
        var botMessages = _configurationService.GetConfigurationObject<BotMessages>();
        var (botClient, chatId, userId, token) = update;
        
        await _repositoryService.CreateUserJoinRequest(userId!.Value);
        await _repositoryService.SaveChanges();
    }
    
    private async Task<bool> UserJoinRequestsIsInLimit(StateUpdate update, int limit)
    {
        var botMessages = _configurationService.GetConfigurationObject<BotMessages>();
        var (botClient, chatId, userId, token) = update;
        
        var userRequests = await _repositoryService.GetUserJoinRequestsByUser(userId!.Value);
        if (userRequests.Count() >= limit) // Limit is reached
        {
            await botClient.SendTextMessageAsync(chatId!, 
                botMessages.JoinRequestLimitReached,
                cancellationToken: token);
            return false;
        }

        return true;
    }

    private async Task<bool> UserIsCompleted(StateUpdate update)
    {
        return (await _repositoryService.GetOrCreateUser(update.UserId!.Value)).IsCompleted;
    }
    
    private Task<bool> UserNameIsValid(StateUpdate update)
    {
        var botMessages = _configurationService.GetConfigurationObject<BotMessages>();
        
        var name = (string)update.Argument!;
        if (name.Length > 30)
        {
            update.ArgumentErrorMessage = botMessages.MessageTooLong;
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }
    
    #endregion

    private async Task SyncUserState(StateUpdate update, UserState state)
    {
        var user = await _repositoryService.GetOrCreateUser(update.UserId!.Value);
        user.State = state;
        await _repositoryService.SaveChanges();
    }
    
    #region Mappers

    #endregion
}