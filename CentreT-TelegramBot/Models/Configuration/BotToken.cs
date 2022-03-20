using System.Diagnostics.CodeAnalysis;

namespace CentreT_TelegramBot.Models.Configuration;

[Attributes.ConfigurationFile]
[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnassignedField.Global")]
public class BotToken
{
    public string? Token;
}