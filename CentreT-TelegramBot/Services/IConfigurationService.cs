namespace CentreT_TelegramBot.Services;

public interface IConfigurationService
{
    T GetConfigurationObject<T>();
    
    object GetConfigurationObject(Type objectType);

    void LoadConfigurationObject<T>();

    void LoadConfigurationObject(Type objectType);
}