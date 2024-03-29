﻿// ReSharper disable InconsistentNaming

using Appccelerate.StateMachine.AsyncMachine;
using Appccelerate.StateMachine.AsyncMachine.Reports;
using CentreT_TelegramBot.Models.States;
using CentreT_TelegramBot.Services;
using CentreT_TelegramBot.StateMachine;
using CentreT_TelegramBot.StateMachine.Reports;
using NUnit.Framework;

namespace CentreT_TelegramBot.Tests;

[TestFixture]
public class BotCoreService_CreateStateMachine
{
    [Test]
    public void CreateStateMachine()
    {
        //var filePath = $"{AppDomain.CurrentDomain.BaseDirectory}/{nameof(BotCoreService)}.graphml";
        var filePath = $"{AppDomain.CurrentDomain.BaseDirectory}/{nameof(BotMenuService)}.gv";
        var service = new BotMenuService(null!, null!, null!, null!, null!, null!);
        var machine = service.CreateStateMachine(UserState.Entry);
        using var sw = new StreamWriter(filePath);
        //machine.Report(new YEdStateMachineReportGenerator<UserState, UserEvent>(sw));
        machine.Report(new DotStateMachineReportGenerator<UserState, UserEvent>(sw));
    }

    [Test]
    public void DoesNotCallInitialStateEntry()
    {
        var builder = new StateMachineDefinitionBuilder<UserState, UserEvent>();
        
        builder.In(UserState.Entry)
            .On(UserEvent.StartCommand)
            .Goto(UserState.Start);
        builder.In(UserState.Entry)
            .ExecuteOnEntry(() => Assert.Fail("ExecuteOnEntry is called"));
        
        builder.WithInitialState(UserState.Entry);
        
        var m = builder.Build().CreatePassiveStateMachine();
        
        m.Load(new SimpleAsyncStateLoader<UserState, UserEvent>(UserState.Entry));
        
        m.Start();
        m.Fire(UserEvent.ArgumentFilled);
    }
}