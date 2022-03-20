using CentreT_TelegramBot.Repositories.Infrastructure;

namespace CentreT_TelegramBot.Repositories;

public class UserContextRepository : GenericRepository<Entities.UserContext>, IUserContextRepository
{
    public UserContextRepository(BotDbContext dbContext) : base(dbContext)
    {
        
    }
}