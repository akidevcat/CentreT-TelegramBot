using CentreT_TelegramBot.Models.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CentreT_TelegramBot.Services;

internal class ConfigurationService : IConfigurationService
{

    private readonly Dictionary<Type, object> _configurationFiles;

    public ConfigurationService(IOptions<ConfigurationServiceOptions> options)
    {
        _configurationFiles = new Dictionary<Type, object>();

        foreach (var type in options.Value.ConfigurationObjects)
        {
            LoadConfigurationObject(type);
        }
    }

    public T GetConfigurationObject<T>() => (T)GetConfigurationObject(typeof(T));

    public object GetConfigurationObject(Type objectType)
    {
        if (_configurationFiles.TryGetValue(objectType, out var result))
            return result;
        throw new ArgumentException("Configuration object was not found", nameof(objectType));
    }
    
    public void LoadConfigurationObject<T>() => LoadConfigurationObject(typeof(T));

    public void LoadConfigurationObject(Type objectType)
    {
        _configurationFiles.Add(objectType, ReadConfigurationFile(objectType));
    }

    public static T ReadConfigurationFile<T>() => (T) ReadConfigurationFile(typeof(T));

    public static object ReadConfigurationFile(Type type)
    {
        var filePath = $"{AppDomain.CurrentDomain.BaseDirectory}/{type.Name}.json";
        if (!File.Exists(filePath))
            throw new FileNotFoundException("File for this object type was not found", filePath);
        var fileContent = File.ReadAllText(filePath);
        var fileObject = JsonConvert.DeserializeObject(fileContent, type);
        return fileObject;
    }
}