using CentreT_TelegramBot.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CentreT_TelegramBot.Repositories;

public class UserJoinRequestRepository : GenericRepository<Models.UserJoinRequest>, IUserJoinRequestRepository
{
    public UserJoinRequestRepository(BotDbContext dbContext) : base(dbContext)
    {
    }
}