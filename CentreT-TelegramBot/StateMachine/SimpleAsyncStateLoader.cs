using Appccelerate.StateMachine;
using Appccelerate.StateMachine.Infrastructure;
using Appccelerate.StateMachine.Persistence;

namespace CentreT_TelegramBot.StateMachine;

public class SimpleAsyncStateLoader<TState, TEvent> : IAsyncStateMachineLoader<TState, TEvent> 
    where TState : IComparable 
    where TEvent : IComparable
{
    private readonly IInitializable<TState> _stateToLoad;

    public SimpleAsyncStateLoader(TState stateToLoad)
    {
        _stateToLoad = Initializable<TState>.Initialized(stateToLoad);
    }

    public Task<IInitializable<TState>> LoadCurrentState()
    {
        return Task.FromResult(_stateToLoad);
    }

    public Task<IReadOnlyDictionary<TState, TState>> LoadHistoryStates()
    {
        // Do not load
        IReadOnlyDictionary<TState, TState> result = new Dictionary<TState, TState>();
        return Task.FromResult(result);
    }

    public Task<IReadOnlyCollection<EventInformation<TEvent>>> LoadEvents()
    {
        // Do not load
        IReadOnlyCollection<EventInformation<TEvent>> result = Array.Empty<EventInformation<TEvent>>();
        return Task.FromResult(result);
    }

    public Task<IReadOnlyCollection<EventInformation<TEvent>>> LoadPriorityEvents()
    {
        // Do not load
        IReadOnlyCollection<EventInformation<TEvent>> result = Array.Empty<EventInformation<TEvent>>();
        return Task.FromResult(result);
    }
}