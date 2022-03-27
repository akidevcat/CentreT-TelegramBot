using Appccelerate.StateMachine;
using Appccelerate.StateMachine.Infrastructure;
using Appccelerate.StateMachine.Persistence;
using CentreT_TelegramBot.Models.States;

namespace CentreT_TelegramBot.StateMachine;

public class SimpleAsyncStateLoader : IAsyncStateMachineLoader<UserState, UserEvent>
{
    private readonly IInitializable<UserState> _stateToLoad;

    public SimpleAsyncStateLoader(UserState stateToLoad)
    {
        _stateToLoad = Initializable<UserState>.Initialized(stateToLoad);
    }

    public Task<IInitializable<UserState>> LoadCurrentState()
    {
        return Task.FromResult(_stateToLoad);
    }

    public Task<IReadOnlyDictionary<UserState, UserState>> LoadHistoryStates()
    {
        // Do not load
        IReadOnlyDictionary<UserState, UserState> result = new Dictionary<UserState, UserState>();
        return Task.FromResult(result);
    }

    public Task<IReadOnlyCollection<EventInformation<UserEvent>>> LoadEvents()
    {
        // Do not load
        IReadOnlyCollection<EventInformation<UserEvent>> result = Array.Empty<EventInformation<UserEvent>>();
        return Task.FromResult(result);
    }

    public Task<IReadOnlyCollection<EventInformation<UserEvent>>> LoadPriorityEvents()
    {
        // Do not load
        IReadOnlyCollection<EventInformation<UserEvent>> result = Array.Empty<EventInformation<UserEvent>>();
        return Task.FromResult(result);
    }
}