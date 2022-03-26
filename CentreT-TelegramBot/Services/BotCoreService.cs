using System.Text.RegularExpressions;
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
        
        _logger.LogError("{ErrorMessage}", errorMessage);
        
        return Task.CompletedTask;
    }

    private void OnTransitionException(object? sender, TransitionExceptionEventArgs<UserState, UserEvent> args)
    {
        _logger.LogError("Exception thrown: {Exception}", args.Exception.ToString());
    }
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    // Ignore all commands
    [ExcludesAnyBackslashCommandFilter]
    [ExcludesCommandsFilter(
        "привет", "информация", "профиль", "редактировать", "чат", "назад", "дальше", "подтверждаю", "подтвердить", "отправить"
        )]
    [ChatTypeFilter(ChatType.Private)]
    protected async Task OnMessage(ITelegramBotClient c, Update u, CancellationToken t)
    {
        if (u.Message == null)
            return;
        
        await EvaluateMachineAction(c, u, t, UserEvent.ArgumentFilled, u.Message.Text);
    }
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    [IncludesCommandsFilter("/start", "привет")]
    [ChatTypeFilter(ChatType.Private)]
    protected async Task OnStartCommand(ITelegramBotClient c, Update u, CancellationToken t)
    {
        await EvaluateMachineAction(c, u, t, UserEvent.StartCommand);
    }
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    [IncludesCommandsFilter("/information", "информация")]
    [ChatTypeFilter(ChatType.Private)]
    protected async Task OnInformationCommand(ITelegramBotClient c, Update u, CancellationToken t)
    {
        await EvaluateMachineAction(c, u, t, UserEvent.InformationCommand);
    }
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    [IncludesCommandsFilter("/profile", "профиль")]
    [ChatTypeFilter(ChatType.Private)]
    protected async Task OnProfileCommand(ITelegramBotClient c, Update u, CancellationToken t)
    {
        await EvaluateMachineAction(c, u, t, UserEvent.ProfileCommand);
    }
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    [IncludesCommandsFilter("/edit", "редактировать")]
    [ChatTypeFilter(ChatType.Private)]
    protected async Task OnEditCommand(ITelegramBotClient c, Update u, CancellationToken t)
    {
        await EvaluateMachineAction(c, u, t, UserEvent.EditCommand);
    }
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    [IncludesCommandsFilter("/join", "чат")]
    [ChatTypeFilter(ChatType.Private)]
    protected async Task OnJoinCommand(ITelegramBotClient c, Update u, CancellationToken t)
    {
        await EvaluateMachineAction(c, u, t, UserEvent.JoinCommand);
    }
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    [IncludesCommandsFilter("/back", "назад")]
    [ChatTypeFilter(ChatType.Private)]
    protected async Task OnBackCommand(ITelegramBotClient c, Update u, CancellationToken t)
    {
        await EvaluateMachineAction(c, u, t, UserEvent.BackCommand);
    }
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    [IncludesCommandsFilter("/next", "дальше")]
    [ChatTypeFilter(ChatType.Private)]
    protected async Task OnNextCommand(ITelegramBotClient c, Update u, CancellationToken t)
    {
        await EvaluateMachineAction(c, u, t, UserEvent.NextCommand);
    }
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    [IncludesCommandsFilter("/confirm", "подтверждаю", "подтвердить", "отправить")]
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
        // ToDo Split into several methods
        
        var m = _configurationService?.GetConfigurationObject<BotMessages>()!;
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
            .ExecuteOnEntry<StateUpdate>(u => 
                ReplyUserWithButtons(u, m.StartMessage, "Информация 📜", "Профиль 👤", "Чат 😸"));
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
                .Execute<StateUpdate>(u => ReplyUser(u, m.JoinRequestCreated))
            .Otherwise()
                .Execute<StateUpdate>(u => ReplyUser(u, m.JoinRequestLimitReached));

        #endregion
        
        #region Profile
        
        // -> Profile
        builder.In(UserState.Profile)
            .ExecuteOnEntry<StateUpdate>(u => 
                ReplyUserWithButtons(u, m.ProfileMessage, "Назад ↩", "Редактировать 👤"));
        // Profile -> Start [/back]
        builder.In(UserState.Profile)
            .On(UserEvent.BackCommand)
            .Goto(UserState.Start);
        // Profile -> ProfileGetName [/edit]
        builder.In(UserState.Profile)
            .On(UserEvent.EditCommand)
            .Goto(UserState.ProfileGetName);
        
        #endregion

        #region ProfileGetName
        
        // -> ProfileGetName
        builder.In(UserState.ProfileGetName)
            .ExecuteOnEntry<StateUpdate>(u => ReplyUserWithButtons(u, m.ProfileGetNameMessage));
        // ProfileGetName -> Profile [/back]
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
                .Execute<StateUpdate>(u => ReplyUser(u, m.InvalidAction));
        // ProfileGetName -> ProfileGetPronouns
        builder.In(UserState.ProfileGetName)
            .On(UserEvent.ArgumentFilled)
            .If<StateUpdate>(ArgumentAsUserNameIsValid)
                .Goto(UserState.ProfileGetPronouns)
                .Execute<StateUpdate>(u => SaveUserPropertyFromArgument(u, UserProperty.Name))
            .Otherwise()
                .Execute<StateUpdate>(u => ReplyUser(u, u.ArgumentErrorMessage!));
        
        #endregion
        
        #region ProfileGetPronouns
        
        // -> ProfileGetPronouns
        builder.In(UserState.ProfileGetPronouns)
            .ExecuteOnEntry<StateUpdate>(u => ReplyUserWithButtons(u, m.ProfileGetPronounsMessage));
        // ProfileGetPronouns -> ProfileGetName [/back]
        builder.In(UserState.ProfileGetPronouns)
            .On(UserEvent.BackCommand)
            .Goto(UserState.ProfileGetName);
        // ProfileGetPronouns -> ProfileGetAge [/next]
        builder.In(UserState.ProfileGetPronouns)
            .On(UserEvent.NextCommand)
            .If<StateUpdate>(u => UserPropertyNotNull(u, UserProperty.Pronouns))
                .Goto(UserState.ProfileGetAge)
            .Otherwise()
                .Execute<StateUpdate>(u => ReplyUser(u, m.InvalidAction));
        // ProfileGetName -> ProfileGetPronouns
        builder.In(UserState.ProfileGetPronouns)
            .On(UserEvent.ArgumentFilled)
            .If<StateUpdate>(ArgumentAsUserPronounsIsValid)
                .Goto(UserState.ProfileGetAge)
                .Execute<StateUpdate>(u => SaveUserPropertyFromArgument(u, UserProperty.Pronouns))
            .Otherwise()
                .Execute<StateUpdate>(u => ReplyUser(u, u.ArgumentErrorMessage!));
        
        #endregion
        
        #region ProfileGetAge
        
        // -> ProfileGetAge
        builder.In(UserState.ProfileGetAge)
            .ExecuteOnEntry<StateUpdate>(u => ReplyUserWithButtons(u, m.ProfileGetAgeMessage));
        // ProfileGetAge -> ProfileGetPronouns [/back]
        builder.In(UserState.ProfileGetAge)
            .On(UserEvent.BackCommand)
            .Goto(UserState.ProfileGetPronouns);
        // ProfileGetAge -> ProfileGetLocation [/next]
        builder.In(UserState.ProfileGetAge)
            .On(UserEvent.NextCommand)
            .If<StateUpdate>(u => UserPropertyNotNull(u, UserProperty.Age))
                .Goto(UserState.ProfileGetLocation)
            .Otherwise()
                .Execute<StateUpdate>(u => ReplyUser(u, m.InvalidAction));
        // ProfileGetAge -> ProfileGetLocation
        builder.In(UserState.ProfileGetAge)
            .On(UserEvent.ArgumentFilled)
            .If<StateUpdate>(ArgumentAsUserAgeIsValid)
                .Goto(UserState.ProfileGetLocation)
                .Execute<StateUpdate>(u => SaveUserPropertyFromArgument(u, UserProperty.Age))
            .Otherwise()
                .Execute<StateUpdate>(u => ReplyUser(u, u.ArgumentErrorMessage!));
        
        #endregion
        
        #region ProfileGetLocation
        
        // -> ProfileGetLocation
        builder.In(UserState.ProfileGetLocation)
            .ExecuteOnEntry<StateUpdate>(u => ReplyUserWithButtons(u, m.ProfileGetLocationMessage));
        // ProfileGetLocation -> ProfileGetAge [/back]
        builder.In(UserState.ProfileGetLocation)
            .On(UserEvent.BackCommand)
            .Goto(UserState.ProfileGetAge);
        // ProfileGetLocation -> Profile [/next]
        builder.In(UserState.ProfileGetLocation)
            .On(UserEvent.NextCommand)
            .If<StateUpdate>(u => UserPropertyNotNull(u, UserProperty.Location))
                .Goto(UserState.Profile)
            .Otherwise()
                .Execute<StateUpdate>(u => ReplyUser(u, m.InvalidAction));
        // ProfileGetLocation -> Profile
        builder.In(UserState.ProfileGetLocation)
            .On(UserEvent.ArgumentFilled)
            .If<StateUpdate>(ArgumentAsUserLocationIsValid)
                .Goto(UserState.Profile)
                .Execute<StateUpdate>(u => SaveUserPropertyFromArgument(u, UserProperty.Location))
            .Otherwise()
                .Execute<StateUpdate>(u => ReplyUser(u, u.ArgumentErrorMessage!));
        
        #endregion
        
        #region JoinConfirmation
        
        // -> JoinConfirmation
        builder.In(UserState.JoinConfirmation)
            .ExecuteOnEntry<StateUpdate>(u => ReplyUserWithButtons(u, m.JoinConfirmationMessage));
        // JoinConfirmation -> JoinConfirmation
        builder.In(UserState.JoinConfirmation)
            .On(UserEvent.ArgumentFilled)
            .If<StateUpdate>(SaveJoinRequestChatFromArgument)
                .Goto(UserState.JoinConfirmation)
            .Otherwise()
                .Execute<StateUpdate>(u => ReplyUser(u, u.ArgumentErrorMessage!));
        // JoinConfirmation -> Start [/back]
        builder.In(UserState.JoinConfirmation)
            .On(UserEvent.BackCommand)
            .Goto(UserState.Start)
            .Execute<StateUpdate>(DeleteUserJoinRequest)
            .Execute<StateUpdate>(u => ReplyUser(u, m.JoinRequestCancelled));
        // JoinConfirmation -> Start [/confirm]
        builder.In(UserState.JoinConfirmation)
            .On(UserEvent.ConfirmCommand)
            .If<StateUpdate>(UserActiveJoinRequestIsCompleted)
                .Goto(UserState.Start)
                .Execute<StateUpdate>(SendUserJoinRequest)
                .Execute<StateUpdate>(u => ReplyUser(u, m.JoinRequestSent))
            .Otherwise()
                .Execute<StateUpdate>(u => ReplyUser(u, m.InvalidAction));
        
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
        var result = await InterpolateMessage(update, message);

        await update.BotClient.SendTextMessageAsync(update.ChatId!, 
            result,
            cancellationToken: update.CancellationToken);
    }
    
    /// <summary>
    /// Replies to user with specified button titles. If buttons are empty, removes keyboard.
    /// </summary>
    private async Task ReplyUserWithButtons(StateUpdate update, string message, params string[] buttons)
    {
        ReplyMarkupBase markup;

        if (buttons.Length == 0)
        {
            markup = new ReplyKeyboardRemove();
        }
        else
        {
            markup = new ReplyKeyboardMarkup(buttons.Select(x => new KeyboardButton(x)))
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }
        
        var result = await InterpolateMessage(update, message);

        await update.BotClient.SendTextMessageAsync(update.ChatId!, 
            result,
            replyMarkup: markup,
            cancellationToken: update.CancellationToken);
    }

    private async Task<string> InterpolateMessage(StateUpdate update, string message)
    {
        var regex = new Regex(@"\{(\w+)\}");
        var (_, _, userId, token) = update;

        var argumentGroups = new Dictionary<string, Dictionary<string, string>>
        {
            { "common",
                new Dictionary<string, string>
                {
                    { "userId", update.UserId?.ToString() ?? "(0_0)" }
                }
            },
            { "user",
                new Dictionary<string, string>
                {
                    { "userName", "(0_0)" },
                    { "userPronouns", "(0_0)" },
                    { "userAge", "(0_0)" },
                    { "userLocation", "(0_0)" }
                }
            },
            { "activeChat",
                new Dictionary<string, string>
                {
                    { "chatId", "(0_0)" },
                    { "chatName", "(0_0)" }
                }
            }
        };

        // ToDo replace with regex somehow
        var flFetchUser = argumentGroups["user"].Any(x => message.Contains($"{{{x.Key}}}"));
        var flFetchActiveChat = argumentGroups["activeChat"].Any(x => message.Contains($"{{{x.Key}}}"));

        if (flFetchUser)
        {
            var user = await _repositoryService.GetOrCreateUser(userId!.Value);
            if (user.Name != null)
                argumentGroups["user"]["userName"] = user.Name;
            if (user.Pronouns != null)
                argumentGroups["user"]["userPronouns"] = user.Pronouns;
            if (user.Age != null)
                argumentGroups["user"]["userAge"] = user.Age.ToString()!;
            if (user.Location != null)
                argumentGroups["user"]["userLocation"] = user.Location;
        }

        if (flFetchActiveChat)
        {
            var activeJoinRequest = await _repositoryService.GetActiveUserJoinRequest(userId!.Value);
            if (activeJoinRequest != null)
            {
                if (activeJoinRequest.ChatId != null)
                    argumentGroups["activeChat"]["chatId"] = activeJoinRequest.ChatId.Value.ToString();
                if (activeJoinRequest.Chat != null)
                    argumentGroups["activeChat"]["chatName"] = activeJoinRequest.Chat.Name;
            }
        }

        var args = argumentGroups
            .SelectMany(x => x.Value)
            .ToDictionary(x => x.Key, x => x.Value);

        return regex.Replace(message, match => args[match.Groups[1].Value]);
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
    
    private async Task<bool> SaveJoinRequestChatFromArgument(StateUpdate update)
    {
        var botMessages = _configurationService.GetConfigurationObject<BotMessages>();
        var (_, _, userId, token) = update;
        
        var user = await _repositoryService.GetOrCreateUser(userId!.Value);
        var request = await _repositoryService.GetActiveUserJoinRequest(userId.Value);
        var arg = (string) update.Argument!;

        if (arg.Length > 30)
        {
            update.ArgumentErrorMessage = botMessages.MessageTooLong;
            return false;
        }

        var chat = await _repositoryService.GetChat(arg);
        if (chat == null)
        {
            update.ArgumentErrorMessage = botMessages.ChatNotFound;
            return false;
        }
        
        var anotherRequest = await _repositoryService.GetUserJoinRequestByChat(chat.Id);
        if (anotherRequest != null && anotherRequest.Id != request.Id)
        {
            update.ArgumentErrorMessage = botMessages.JoinRequestAlreadyExists;
            return false;
        }

        request!.Chat = chat;
        await _repositoryService.SaveChanges();
        return true;
    }

    private async Task DeleteUserJoinRequest(StateUpdate update)
    {
        var (_, _, userId, token) = update;
        await _repositoryService.DeleteActiveUserJoinRequest(userId!.Value);
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

    private async Task<bool> UserActiveJoinRequestIsCompleted(StateUpdate update)
    {
        var botMessages = _configurationService.GetConfigurationObject<BotMessages>();
        var (_, _, userId, token) = update;
        
        var request = await _repositoryService.GetActiveUserJoinRequest(userId!.Value);

        return request != null && request.Completed;
    }

    private async Task SendUserJoinRequest(StateUpdate update)
    {
        var botMessages = _configurationService.GetConfigurationObject<BotMessages>();
        var (botClient, chatId, userId, token) = update;
        
        var request = await _repositoryService.GetActiveUserJoinRequest(userId!.Value);
        request!.DateCreated = DateTime.Now;
        await _repositoryService.SaveChanges();
    }

    private async Task CreateUserJoinRequest(StateUpdate update)
    {
        var botMessages = _configurationService.GetConfigurationObject<BotMessages>();
        var (botClient, chatId, userId, token) = update;
        
        await _repositoryService.GetOrCreateActiveUserJoinRequest(userId!.Value);
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
    
    private Task<bool> ArgumentAsUserNameIsValid(StateUpdate update)
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

    private Task<bool> ArgumentAsUserPronounsIsValid(StateUpdate update)
    {
        var botMessages = _configurationService.GetConfigurationObject<BotMessages>();
        
        var pronouns = (string)update.Argument!;
        if (pronouns.Length > 15)
        {
            update.ArgumentErrorMessage = botMessages.MessageTooLong;
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }
    
    private Task<bool> ArgumentAsUserAgeIsValid(StateUpdate update)
    {
        var botMessages = _configurationService.GetConfigurationObject<BotMessages>();
        
        var age = (string)update.Argument!;
        if (!short.TryParse(age, out var ageValue))
        {
            update.ArgumentErrorMessage = botMessages.InvalidAge;
            return Task.FromResult(false);
        }
        
        if (ageValue is < 14 or > 100)
        {
            update.ArgumentErrorMessage = botMessages.InvalidAge;
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }
    
    private Task<bool> ArgumentAsUserLocationIsValid(StateUpdate update)
    {
        var botMessages = _configurationService.GetConfigurationObject<BotMessages>();
        
        var location = (string)update.Argument!;
        if (location.Length > 30)
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