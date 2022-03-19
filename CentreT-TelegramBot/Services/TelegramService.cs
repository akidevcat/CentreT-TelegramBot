using System.Reflection;
using CentreT_TelegramBot.Attributes.Telegram.Bot;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CentreT_TelegramBot.Services;

public class TelegramService : ITelegramService
{
    private readonly List<(IUpdateHandler, MethodInfo)> _updateHandlerMethods;
    private readonly List<(IErrorHandler, MethodInfo)> _errorHandlerMethods;
    
    public TelegramService(IEnumerable<IUpdateHandler> updateHandlers, IEnumerable<IErrorHandler> errorHandlers)
    {
        _updateHandlerMethods = new List<(IUpdateHandler, MethodInfo)>();
        _errorHandlerMethods = new List<(IErrorHandler, MethodInfo)>();

        // ToDo add arguments check

        _updateHandlerMethods.AddRange(ScanHandlerMethods<IUpdateHandler, UpdateHandlerAttribute>(updateHandlers));
        _errorHandlerMethods.AddRange(ScanHandlerMethods<IErrorHandler, ErrorHandlerAttribute>(errorHandlers));
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var arguments = new object[] { botClient, exception, cancellationToken };
        
        foreach (var (invoker, methodInfo) in _errorHandlerMethods)
        {
            await (Task)methodInfo.Invoke(invoker, arguments)!;
        }
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var arguments = new object[] { botClient, update, cancellationToken };
        
        foreach (var (invoker, methodInfo) in _updateHandlerMethods)
        {
            await (Task)methodInfo.Invoke(invoker, arguments)!;
        }
    }

    private static IEnumerable<(THandler, MethodInfo)> ScanHandlerMethods<THandler, TAttribute>(IEnumerable<THandler> handlers)
    {
        var result = new List<(THandler, MethodInfo)>();
        
        foreach (var handler in handlers)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handlers));
            
            var methods = handler.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | 
                                     BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.GetCustomAttributes(typeof(TAttribute), true).Length > 0);

            result.AddRange(methods.Select(method => (handler, method)));
        }

        return result;
    }
}