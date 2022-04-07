using Appccelerate.StateMachine;
using Appccelerate.StateMachine.AsyncMachine;
using Appccelerate.StateMachine.AsyncMachine.Events;
using CentreT_TelegramBot.Exceptions;
using CentreT_TelegramBot.StateMachine;

namespace CentreT_TelegramBot.Services;

public abstract class BotStateMachineService<TState, TEvent> : BotGenericService
    where TState : Enum
    where TEvent : Enum
{
    protected StateMachineDefinition<TState, TEvent>? StateMachineDefinition { get; private set; }

    protected virtual void OnTransitionException(object? sender, TransitionExceptionEventArgs<TState, TEvent> args)
    {
        
    }
    
    protected virtual void OnTransitionDeclined(object? sender, TransitionEventArgs<TState, TEvent> e)
    {
        
    }
    
    protected internal AsyncPassiveStateMachine<TState, TEvent> CreateStateMachine(TState state, string name = "NoName")
    {
        if (StateMachineDefinition == null)
        {
            throw new StateMachineNotInitializedException();
        }
        
        var machine = StateMachineDefinition.CreatePassiveStateMachine(name);
        
        machine.TransitionExceptionThrown += OnTransitionException;
        machine.TransitionDeclined += OnTransitionDeclined;

        machine.Load(new SimpleAsyncStateLoader<TState, TEvent>(state));
        machine.Start();

        return machine;
    }
    
    protected void BuildMachine(TState initialState, params Action<StateMachineDefinitionBuilder<TState, TEvent>>[] stateInitializers)
    {
        var builder = new StateMachineDefinitionBuilder<TState, TEvent>();

        // Add all states
        foreach (var initializer in stateInitializers)
        {
            initializer(builder);
        }

        builder.WithInitialState(initialState);

        StateMachineDefinition = builder.Build();
    }
}