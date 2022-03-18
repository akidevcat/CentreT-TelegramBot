using CentreT_TelegramBot.Attributes;
using CentreT_TelegramBot.Services;

namespace CentreT_TelegramBot.Extensions;

public static class ConfigurationServiceCollectionExtensions
{
    public static IServiceCollection AddConfigurationFile<T>(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.Configure<ConfigurationServiceOptions>(x => x.Add(typeof(T)));

        return services;
    }
    
    public static IServiceCollection AddConfigurationFiles(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var configurationFileTypes =
            from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetTypes()
            let attributes = type.GetCustomAttributes(typeof(ConfigurationFileAttribute), true)
            where attributes is { Length: > 0 }
            select type;

        foreach (var type in configurationFileTypes)
        {
            services.Configure<ConfigurationServiceOptions>(x => x.Add(type));
        }
        
        return services;
    }
}