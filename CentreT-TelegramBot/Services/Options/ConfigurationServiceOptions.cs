namespace CentreT_TelegramBot.Services;

public class ConfigurationServiceOptions : List<Type>
{
    public IEnumerable<Type> ConfigurationObjects => this;
}