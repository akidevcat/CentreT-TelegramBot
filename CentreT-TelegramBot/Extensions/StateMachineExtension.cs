using Appccelerate.StateMachine;
using Appccelerate.StateMachine.Extensions;

namespace CentreT_TelegramBot.Extensions;

public class StateMachineExtension<TState, TEvent> : ExtensionBase<TState, TEvent> 
    where TState : IComparable 
    where TEvent : IComparable
{
    //EnteringState
    //EnteringInitialState
    //EventQueued
    //StartedStateMachine
}