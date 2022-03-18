using CentreT_TelegramBot;
using CentreT_TelegramBot.Extensions;
using CentreT_TelegramBot.Services;

var hostBuilder = Host.CreateDefaultBuilder(args);

hostBuilder.ConfigureServices(services =>
{
    services.AddHostedService<Worker>();
    services.AddSingleton<IConfigurationService, ConfigurationService>();
    // Add configuration files
    services.AddConfigurationFiles();
});

var host = hostBuilder.Build();
await host.RunAsync();
