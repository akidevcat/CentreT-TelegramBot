using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CentreT_TelegramBot.Extensions;

public static class ServiceCollectionRegistrationExtensions
{

    /// <summary>
    /// Adds multiple services mapped to a single instance of <see cref="TImplementation"/>.
    /// </summary>
    public static void AddSingletonMultiple<TImplementation>(
        this IServiceCollection serviceCollection, 
        params Type[] services) 
        where TImplementation : notnull
    {
        if (services.Length == 0)
            return;
        
        serviceCollection.AddSingleton(typeof(TImplementation));

        foreach (var service in services)
        {
            serviceCollection.AddSingleton(service, x => x.GetRequiredService<TImplementation>());
        }
    }
    
    public static void RegisterAllImplementationsOf<T>(
        this IServiceCollection serviceCollection,
        Action<IServiceCollection, Type, Func<IServiceProvider, object>> registrationFunc, 
        params Assembly[] assemblies)
    {
        var targetType = typeof(T);
        var types = assemblies
            .Distinct()
            .SelectMany(a => a.GetTypes())
            //.Where(t => !t.IsInterface && !t.IsAbstract && t.GetInterfaces().Contains(targetType));
            .Where(t => !t.IsInterface && !t.IsAbstract && t.IsAssignableTo(targetType));

        foreach (var type in types)
        {
            registrationFunc(serviceCollection, targetType, x => x.GetRequiredService(type));
        }
    }
}