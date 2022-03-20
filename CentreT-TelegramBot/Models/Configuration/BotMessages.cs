using System.Diagnostics.CodeAnalysis;

namespace CentreT_TelegramBot.Models.Configuration;

[Attributes.ConfigurationFile]
[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnassignedField.Global")]
public class BotMessages
{
    public string InformationMessage = "596093EF-710C-4246-BAEC-723633B76C42";
    public string JoinMessage = "503870EC-C32E-4B04-BB0A-1FD66AB9CD0D";
}