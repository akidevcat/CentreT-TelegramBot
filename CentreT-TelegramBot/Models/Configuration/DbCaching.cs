using System.Diagnostics.CodeAnalysis;

namespace CentreT_TelegramBot.Models.Configuration;

[Attributes.ConfigurationFile]
[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnassignedField.Global")]
public class DbCaching
{
    public bool EnableCaching = false;
    public int AbsoluteExpirationInHours = 1;
}