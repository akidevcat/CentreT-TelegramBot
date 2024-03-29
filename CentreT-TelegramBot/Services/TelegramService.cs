﻿using System.Reflection;
using CentreT_TelegramBot.Attributes.Telegram.Bot;
using CentreT_TelegramBot.Extensions;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace CentreT_TelegramBot.Services;

public class TelegramService : ITelegramService
{
    private readonly ITelegramContext _context;
    private readonly ILogger<TelegramService> _logger;
    
    private readonly List<(IUpdateHandler, MethodInfo)> _updateHandlerMethods;
    private readonly List<(IErrorHandler, MethodInfo)> _errorHandlerMethods;

    public TelegramService(IEnumerable<IUpdateHandler> updateHandlers, IEnumerable<IErrorHandler> errorHandlers, ITelegramContext context,
        ILogger<TelegramService> logger)
    {
        _context = context;
        _logger = logger;
        
        _updateHandlerMethods = new List<(IUpdateHandler, MethodInfo)>();
        _errorHandlerMethods = new List<(IErrorHandler, MethodInfo)>();

        _updateHandlerMethods.AddRange(ScanHandlerMethods<IUpdateHandler, UpdateHandlerAttribute>(
            updateHandlers, typeof(Task), typeof(ITelegramBotClient), typeof(Update), typeof(CancellationToken)));
        _errorHandlerMethods.AddRange(ScanHandlerMethods<IErrorHandler, ErrorHandlerAttribute>(
            errorHandlers, typeof(Task), typeof(ITelegramBotClient), typeof(Exception), typeof(CancellationToken)));
    }

    public void Run(string botToken, CancellationToken cancellationToken)
    {
        if (_context.BotClient != null)
            throw new InvalidOperationException($"New bot execution was called but bot is already running.");
        
        _context.BotClient = new TelegramBotClient(botToken);
        ReceiverOptions receiverOptions = new() { };

        // Start polling with telegram service
        _context.BotClient.StartReceiving(
            HandleUpdateAsync, 
            HandleErrorAsync, 
            receiverOptions, cancellationToken);
    }

    private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var arguments = new object[] { botClient, exception, cancellationToken };
        
        foreach (var (invoker, methodInfo) in _errorHandlerMethods)
        {
            await (Task)methodInfo.Invoke(invoker, arguments)!;
        }
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var arguments = new object[] { botClient, update, cancellationToken };

        foreach (var (invoker, methodInfo) in _updateHandlerMethods)
        {
            // Get all attributes that are not appliable for this call
            var leftAttribute = methodInfo.GetCustomAttributes().FirstOrDefault(x =>
                x switch
                {
                    UpdateTypeFilterAttribute a => !update.IsOfType(a.UpdateType),
                    ChatTypeFilterAttribute a => !update.IsChatOfType(a.ChatType),
                    IncludesCommandsFilterAttribute a => !update.StartsWithCommands(a.Commands),
                    ExcludesCommandsFilterAttribute a => !(update.StartsWithCommands(a.Commands) ^ true),
                    ExcludesAnyBackslashCommandFilterAttribute => update.StartsWithBackslash(),
                    HasArgumentsFilterAttribute a => !update.HasSeveralArguments(a.ArgumentCount),
                    FromNotNullFilterAttribute => update.IsMessageFromNull(),
                    FromBotFilterAttribute a => a.ShouldBeBot != update.IsMessageFromBot(),
                    _ => false // Skip general attributes
                });
            
            // If attributes are left - we should not invoke this method
            if (leftAttribute == null)
                await (Task)methodInfo.Invoke(invoker, arguments)!;
        }
    }

    private static IEnumerable<(THandler, MethodInfo)> ScanHandlerMethods<THandler, TAttribute>(
        IEnumerable<THandler> handlers, Type returnType, params Type[] argumentTypes)
    {
        var result = new List<(THandler, MethodInfo)>();
        
        foreach (var handler in handlers)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handlers));

            var methods = handler.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic |
                            BindingFlags.Public | BindingFlags.Static)
                // Get only methods with the required TAttribute
                .Where(m => m.GetCustomAttributes(typeof(TAttribute), true).Length > 0)
                // Check for return type to be assignable
                .Where(m => m.ReturnType.IsAssignableTo(returnType))
                // Check for arguments to be as stated in argumentTypes
                .Where(m => m.GetParameters().Select(p => p.ParameterType).SequenceEqual(argumentTypes));

            result.AddRange(methods.Select(method => (handler, method)));
        }

        return result;
    }
}