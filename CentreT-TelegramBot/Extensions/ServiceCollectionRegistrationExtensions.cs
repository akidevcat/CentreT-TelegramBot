using System.Reflection;

namespace CentreT_TelegramBot.Extensions;

public static class ServiceCollectionRegistrationExtensions
{
    public static void RegisterAllImplementationsOf<T>(
        this IServiceCollection serviceCollection,
        Action<IServiceCollection, Type, Type> registrationFunc, 
        params Assembly[] assemblies)
    {
        var targetType = typeof(T);
        var types = assemblies
            .Distinct()
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsInterface && !t.IsAbstract && t.GetInterfaces().Contains(targetType));

        foreach (var type in types)
        {
            registrationFunc(serviceCollection, targetType, type);
        }
    }
}