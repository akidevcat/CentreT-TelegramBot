using CentreT_TelegramBot.Attributes.Telegram.Bot;
using CentreT_TelegramBot.Models.States;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CentreT_TelegramBot.Services;

public class BotAdminService : BotStateMachineService<UserState, UserEvent>, IBotAdminService
{
    
    public BotAdminService()
    {
        
    }
    
    public override Task RunAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    
    [UpdateHandler]
    [UpdateTypeFilter(UpdateType.Message)]
    [IncludesCommandsFilter("/register")]
    [ChatTypeFilter(ChatType.Group)]
    protected async Task OnRegisterCommand(ITelegramBotClient c, Update u, CancellationToken t)
    {
        
    }
    
}