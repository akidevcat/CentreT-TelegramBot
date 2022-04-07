using CentreT_TelegramBot;
using CentreT_TelegramBot.Extensions;
using CentreT_TelegramBot.Models.Configuration;
using CentreT_TelegramBot.Repositories;
using CentreT_TelegramBot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
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
    
    // Add database services
    services.AddDbContext<BotDbContext>(options => options.UseSqlServer(dbConnection.ConnectionString), ServiceLifetime.Singleton);
    services.TryAddSingleton<IUserRepository, UserRepository>();
    services.TryAddSingleton<IChatRepository, ChatRepository>();
    services.TryAddSingleton<IUserJoinRequestRepository, UserJoinRequestRepository>();
    services.TryAddSingleton<IRepositoryService, RepositoryService>();
    
    // Add telegram service
    services.TryAddSingleton<ITelegramService, TelegramService>();
    services.TryAddSingleton<ITelegramContext, TelegramContext>();
    
    // Add bot services
    services.AddSingletonMultiple<BotMenuService>(
        typeof(IBotMenuService), 
        typeof(IUpdateHandler), 
        typeof(IErrorHandler));
    services.AddSingletonMultiple<BotGroupService>(
        typeof(IBotGroupService), 
        typeof(IUpdateHandler), 
        typeof(IErrorHandler));

    // Add hosted
    services.AddHostedService<Worker>();
});

var host = hostBuilder.Build();
await host.RunAsync();
