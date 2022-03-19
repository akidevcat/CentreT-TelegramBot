using System.Reflection;
using CentreT_TelegramBot;
using CentreT_TelegramBot.Extensions;
using CentreT_TelegramBot.Models.Configuration;
using CentreT_TelegramBot.Repositories;
using CentreT_TelegramBot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

var dbConnection = ConfigurationService.ReadConfigurationFile<DbConnection>();
if (dbConnection.ConnectionString == null)
    throw new ApplicationException(
        $"Configuration file {nameof(DbConnection)}.json has no definition for {nameof(dbConnection.ConnectionString)}. Aborting.");

var hostBuilder = Host.CreateDefaultBuilder(args);

hostBuilder.ConfigureServices(services =>
{
    // Add configuration services
    services.AddConfigurationFiles();
    // Add databases services
    services.AddDbContext<BotDbContext>(options => options.UseSqlServer(dbConnection.ConnectionString));
    services.TryAddScoped<IUserRepository, UserRepository>();
    // Add telegram service
    services.TryAddSingleton<ITelegramService, TelegramService>();
    services.TryAddSingleton<ITelegramContext, TelegramContext>();
    // Add bot services
    services.TryAddSingleton<IBotCoreService, BotCoreService>();
    
    // Register all Update and Error Handlers for Telegram.Bot
    services.RegisterAllImplementationsOf<IUpdateHandler>(ServiceCollectionDescriptorExtensions.TryAddSingleton, 
        typeof(IUpdateHandler).Assembly);
    services.RegisterAllImplementationsOf<IErrorHandler>(ServiceCollectionDescriptorExtensions.TryAddSingleton, 
        typeof(IErrorHandler).Assembly);

    // Add hosted
    services.AddHostedService<Worker>();
});

var host = hostBuilder.Build();
await host.RunAsync();
