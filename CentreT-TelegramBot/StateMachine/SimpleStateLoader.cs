using Appccelerate.StateMachine;
using Appccelerate.StateMachine.Infrastructure;
using Appccelerate.StateMachine.Persistence;
using CentreT_TelegramBot.Entities.States;

namespace CentreT_TelegramBot.StateMachine;

public class SimpleStateLoader : IStateMachineLoader<UserState, UserEvent>
{
    private readonly Initializable<UserState> _stateToLoad;

    public SimpleStateLoader(UserState stateToLoad)
    {
        _stateToLoad = Initializable<UserState>.Initialized(stateToLoad);
    }
    
    public IInitializable<UserState> LoadCurrentState()
    {
        return _stateToLoad;
    }

    public IReadOnlyDictionary<UserState, UserState> LoadHistoryStates()
    {
        // Do not load
        return new Dictionary<UserState, UserState>();
    }

    public IReadOnlyCollection<EventInformation<UserEvent>> LoadEvents()
    {
        // Do not load
        return Array.Empty<EventInformation<UserEvent>>();
    }
}