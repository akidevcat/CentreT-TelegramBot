﻿using System.Text;
using System.Text.RegularExpressions;
using Appccelerate.StateMachine;
using Appccelerate.StateMachine.AsyncMachine;
using Appccelerate.StateMachine.AsyncMachine.Events;
using CentreT_TelegramBot.Attributes.Telegram.Bot;
using CentreT_TelegramBot.Models.States;
using CentreT_TelegramBot.Models.Configuration;
using CentreT_TelegramBot.Extensions;
using CentreT_TelegramBot.Models;
using CentreT_TelegramBot.Repositories;
using CentreT_TelegramBot.StateMachine;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CentreT_TelegramBot.Services;

public class BotMenuService : BotStateMachineService<UserState, UserEvent>, IBotMenuService
{
    private readonly ITelegramContext _telegramContext;
    private readonly IUserRepository _userRepository;
    private readonly IUserJoinRequestRepository _userJoinRequestRepository;
    private readonly IChatRepository _chatRepository;
    private readonly IConfigurationService _configurationService;
    private readonly ILogger<BotMenuService> _logger;
    
    public BotMenuService(IUserRepository userRepository, IUserJoinRequestRepository userJoinRequestRepository,
        IChatRepository chatRepository,
        ITelegramContext telegramContext, IConfigurationService configurationService, 
        ILogger<BotMenuService> logger)
    {
        _userRepository = userRepository;
        _userJoinRequestRepository = userJoinRequestRepository;
        _chatRepository = chatRepository;
        _telegramContext = telegramContext;
        _configurationService = configurationService;
        _logger = logger;

        var m = _configurationService?.GetConfigurationObject<BotMessages>()!;
        
        BuildMachine(UserState.Entry,
            b => AddEntryStateToMachine(b, m),
            b => AddStartStateToMachine(b, m),
            b => AddProfileStateToMachine(b, m),
            b => AddProfileGetNameStateToMachine(b, m),
            b => AddProfileGetPronounsStateToMachine(b, m),
            b => AddProfileGetAgeStateToMachine(b, m),
            b => AddProfileGetLocationStateToMachine(b, m),
            b => AddJoinConfirmationStateToMachine(b, m),
            b => b.AddStateSync<UserState, UserEvent, StateUpdate>(SyncUserState)
        );
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

    protected override void OnTransitionException(object? sender, TransitionExceptionEventArgs<UserState, UserEvent> args)
    {
        _logger.LogError("Exception thrown: {Exception}", args.Exception.ToString());
    }
    
    #region Commands
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    // Ignore all commands
    [ExcludesAnyBackslashCommandFilter]
    [ExcludesCommandsFilter(
        "привет", "начать", "информация", "профиль", "редактировать", "чат", "назад", "дальше", "подтверждаю", "подтвердить", "отправить"
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
    [IncludesCommandsFilter("/start", "привет", "начать")]
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
    
    #endregion

    private async Task EvaluateMachineAction(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, 
        UserEvent userEvent, object? argument = null)
    {
        var userId = update.Message!.From!.Id;
        var user = await _userRepository.GetOrCreate(userId);
        var stateUpdate = new StateUpdate(botClient, update, cancellationToken);
        var machine = CreateStateMachine(user.State, userId.ToString());

        stateUpdate.Argument = argument;
        await machine.Fire(userEvent, stateUpdate);
    }

    #region User StateMachine Definition
    
    private void AddEntryStateToMachine(StateMachineDefinitionBuilder<UserState, UserEvent> builder, BotMessages m)
    {
        // Entry -> ProfileGetName
        builder.In(UserState.Entry)
            .On(UserEvent.StartCommand)
            .Goto(UserState.ProfileGetName)
            .Execute<StateUpdate>(u => ReplyUser(u, m.EntryMessage));
    }
    
    private void AddStartStateToMachine(StateMachineDefinitionBuilder<UserState, UserEvent> builder, BotMessages m)
    {
        // -> Start
        builder.In(UserState.Start)
            .ExecuteOnEntry<StateUpdate>(u => 
                ReplyUser(u, m.StartMessage, buttons: new [] { "Информация 📜", "Профиль 👤", "Чат 😸" }));
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
    }

    private void AddProfileStateToMachine(StateMachineDefinitionBuilder<UserState, UserEvent> builder, BotMessages m)
    {
        // -> Profile
        builder.In(UserState.Profile)
            .ExecuteOnEntry<StateUpdate>(u => 
                ReplyUser(u, m.ProfileMessage, buttons: new[] { "Назад ↩", "Редактировать 👤" }));
        // Profile -> Start [/back]
        builder.In(UserState.Profile)
            .On(UserEvent.BackCommand)
            .Goto(UserState.Start);
        // Profile -> ProfileGetName [/edit]
        builder.In(UserState.Profile)
            .On(UserEvent.EditCommand)
            .Goto(UserState.ProfileGetName);
    }

    private void AddProfileGetNameStateToMachine(StateMachineDefinitionBuilder<UserState, UserEvent> builder,
        BotMessages m)
    {
        // -> ProfileGetName
        builder.In(UserState.ProfileGetName)
            .ExecuteOnEntry<StateUpdate>(u => ReplyUser(u, m.ProfileGetNameMessage, buttons: Array.Empty<string>()));
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
    }
    
    private void AddProfileGetPronounsStateToMachine(StateMachineDefinitionBuilder<UserState, UserEvent> builder,
        BotMessages m)
    {
        // -> ProfileGetPronouns
        builder.In(UserState.ProfileGetPronouns)
            .ExecuteOnEntry<StateUpdate>(u => ReplyUser(u, m.ProfileGetPronounsMessage, buttons: Array.Empty<string>()));
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
    }
    
    private void AddProfileGetAgeStateToMachine(StateMachineDefinitionBuilder<UserState, UserEvent> builder,
        BotMessages m)
    {
        // -> ProfileGetAge
        builder.In(UserState.ProfileGetAge)
            .ExecuteOnEntry<StateUpdate>(u => ReplyUser(u, m.ProfileGetAgeMessage, buttons: Array.Empty<string>()));
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
    }
    
    private void AddProfileGetLocationStateToMachine(StateMachineDefinitionBuilder<UserState, UserEvent> builder,
        BotMessages m)
    {
        // -> ProfileGetLocation
        builder.In(UserState.ProfileGetLocation)
            .ExecuteOnEntry<StateUpdate>(u => ReplyUser(u, m.ProfileGetLocationMessage, buttons: Array.Empty<string>()));
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
    }
    
    private void AddJoinConfirmationStateToMachine(StateMachineDefinitionBuilder<UserState, UserEvent> builder,
        BotMessages m)
    {
        // -> JoinConfirmation
        builder.In(UserState.JoinConfirmation)
            .ExecuteOnEntry<StateUpdate>(u => ReplyUser(u, m.JoinConfirmationMessage, buttons: Array.Empty<string>()));
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
    }

    #endregion

    #region Actions / Guards

    protected override void OnTransitionDeclined(object? sender, TransitionEventArgs<UserState, UserEvent> e)
    {
        var botMessages = _configurationService.GetConfigurationObject<BotMessages>();
        var update = (StateUpdate)e.EventArgument;
        // Exception is handled by the bot API
        update.BotClient.SendTextMessageAsync(update.ChatId!, 
            botMessages.InvalidAction,
            cancellationToken: update.CancellationToken);
    }

    

    private async Task<string> InterpolateMessage(StateUpdate update, string message)
    {
        // ToDo make something more readable and modular
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
            },
            { "allChats",
                new Dictionary<string, string>
                {
                    { "chats", "(0_0)" }
                }
            }
        };

        // ToDo replace with regex somehow
        var flFetchUser = argumentGroups["user"].Any(x => message.Contains($"{{{x.Key}}}"));
        var flFetchActiveChat = argumentGroups["activeChat"].Any(x => message.Contains($"{{{x.Key}}}"));
        var flFetchChats = argumentGroups["allChats"].Any(x => message.Contains($"{{{x.Key}}}"));

        if (flFetchUser)
        {
            var user = await _userRepository.GetOrCreate(userId!.Value);
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
            var activeJoinRequest = await _userJoinRequestRepository.GetActive(userId!.Value);
            if (activeJoinRequest != null)
            {
                if (activeJoinRequest.ChatId != null)
                    argumentGroups["activeChat"]["chatId"] = activeJoinRequest.ChatId.Value.ToString();
                if (activeJoinRequest.Chat != null)
                    argumentGroups["activeChat"]["chatName"] = activeJoinRequest.Chat.Name;
            }
        }

        if (flFetchChats)
        {
            var chats = await _chatRepository.GetAllChats();
            var result = new StringBuilder();
            foreach (var chat in chats)
            {
                result.AppendLine(chat.Name);
            }

            argumentGroups["allChats"]["chats"] = result.ToString();
        }

        var args = argumentGroups
            .SelectMany(x => x.Value)
            .ToDictionary(x => x.Key, x => x.Value);

        return regex.Replace(message, match => args[match.Groups[1].Value]);
    }

    private async Task SaveUserPropertyFromArgument(StateUpdate update, UserProperty property)
    {
        var (_, _, userId, token) = update;
        
        var user = await _userRepository.GetOrCreate(userId!.Value);
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

        await _userRepository.Save();
    }
    
    private async Task<bool> SaveJoinRequestChatFromArgument(StateUpdate update)
    {
        var botMessages = _configurationService.GetConfigurationObject<BotMessages>();
        var (_, _, userId, token) = update;
        
        var user = await _userRepository.GetOrCreate(userId!.Value);
        var request = await _userJoinRequestRepository.GetActive(userId.Value);
        var arg = (string) update.Argument!;

        if (arg.Length > 30)
        {
            update.ArgumentErrorMessage = botMessages.MessageTooLong;
            return false;
        }

        var chat = await _chatRepository.GetChat(arg);
        if (chat == null)
        {
            update.ArgumentErrorMessage = botMessages.ChatNotFound;
            return false;
        }
        
        //var anotherRequest = await _userJoinRequestRepository.GetUserJoinRequestByChat(chat.Id);
        var existingRequest = await _userJoinRequestRepository.Get(userId.Value, chat.Id);
        if (existingRequest != null && existingRequest.Id != request.Id)
        {
            update.ArgumentErrorMessage = botMessages.JoinRequestAlreadyExists;
            return false;
        }

        request!.Chat = chat;
        await _userJoinRequestRepository.Save();
        return true;
    }

    private async Task DeleteUserJoinRequest(StateUpdate update)
    {
        var (_, _, userId, token) = update;
        await _userJoinRequestRepository.DeleteActive(userId!.Value);
    }
    
    private async Task<bool> UserPropertyNotNull(StateUpdate update, UserProperty property)
    {
        var botMessages = _configurationService.GetConfigurationObject<BotMessages>();
        var (_, _, userId, token) = update;
        
        var user = await _userRepository.GetOrCreate(userId!.Value);

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
        
        var request = await _userJoinRequestRepository.GetActive(userId!.Value);

        return request != null && request.Completed;
    }

    private async Task SendUserJoinRequest(StateUpdate update)
    {
        var botMessages = _configurationService.GetConfigurationObject<BotMessages>();
        var (botClient, chatId, userId, token) = update;
        
        var request = await _userJoinRequestRepository.GetActive(userId!.Value);
        request!.DateCreated = DateTime.Now;
        await _userJoinRequestRepository.Save();
    }

    private async Task CreateUserJoinRequest(StateUpdate update)
    {
        var botMessages = _configurationService.GetConfigurationObject<BotMessages>();
        var (botClient, chatId, userId, token) = update;
        
        await _userJoinRequestRepository.CreateActive(userId!.Value);
        await _userJoinRequestRepository.Save();
    }
    
    private async Task<bool> UserJoinRequestsIsInLimit(StateUpdate update, int limit)
    {
        var botMessages = _configurationService.GetConfigurationObject<BotMessages>();
        var (botClient, chatId, userId, token) = update;
        
        var userRequests = await _userJoinRequestRepository.GetByUserId(userId!.Value);
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
        return (await _userRepository.GetOrCreate(update.UserId!.Value)).IsCompleted;
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
        var user = await _userRepository.GetOrCreate(update.UserId!.Value);
        user.State = state;
        await _userRepository.Save();
    }
}