using Appccelerate.StateMachine;
using Appccelerate.StateMachine.Infrastructure;
using Appccelerate.StateMachine.Persistence;
using CentreT_TelegramBot.Entities.States;

namespace CentreT_TelegramBot.StateMachine;

public class SimpleStateSaver : IStateMachineSaver<UserState, UserEvent>
{
    public UserState State { get; private set; }

    public void SaveCurrentState(IInitializable<UserState> currentStateId)
    {
        State = currentStateId.ExtractOrThrow(); // Only if initialized; else throw
    }

    public void SaveHistoryStates(IReadOnlyDictionary<UserState, UserState> historyStates)
    {
        // Skip; do not save
    }

    public void SaveEvents(IReadOnlyCollection<EventInformation<UserEvent>> events)
    {
        // Skip; do not save
    }
}