using Appccelerate.StateMachine.AsyncMachine;

namespace CentreT_TelegramBot.Extensions;

public static class StateMachineDefinitionBuilderExtensions
{
    public static void AddStateSync<TState, TEvent, TArg>(this StateMachineDefinitionBuilder<TState, TEvent> builder, 
        Func<TArg, TState, Task> syncFunc)
        where TState : Enum
        where TEvent : Enum
    {
        foreach (var state in (TState[])Enum.GetValues(typeof(TState)))
        {
            builder.In(state)
                .ExecuteOnEntry<TArg>(a => syncFunc(a, state));
        }
    }
}