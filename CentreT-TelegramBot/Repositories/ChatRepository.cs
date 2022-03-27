using CentreT_TelegramBot.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CentreT_TelegramBot.Repositories;

public class ChatRepository : GenericRepository<Models.Chat>, IChatRepository
{
    public ChatRepository(BotDbContext dbContext) : base(dbContext)
    {
    }
}