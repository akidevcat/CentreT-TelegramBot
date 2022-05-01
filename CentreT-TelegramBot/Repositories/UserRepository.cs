using CentreT_TelegramBot.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CentreT_TelegramBot.Repositories;

public class UserRepository : GenericRepository<Models.User>, IUserRepository
{
    public UserRepository(BotDbContext dbContext) : base(dbContext)
    {
        
    }

    public async Task<Models.User> GetOrCreate(long userId)
    {
        return await GetOrCreate(new Models.User(userId), true, userId);
    }
}