using CentreT_TelegramBot.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CentreT_TelegramBot.Repositories;

public class UserJoinRequestRepository : GenericRepository<Models.UserJoinRequest>, IUserJoinRequestRepository
{
    public UserJoinRequestRepository(BotDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Models.UserJoinRequest?> CreateActive(long userId)
    {
        return await Create(new Models.UserJoinRequest(userId));
    }

    public async Task<Models.UserJoinRequest?> GetActive(long userId)
    {
        return await GetFirst(x =>
            x.UserId == userId && x.DateCreated == null);
    }

    public async Task<Models.UserJoinRequest?> Get(long userId, long chatId)
    {
        return await GetFirst(x =>
            x.UserId == userId && x.ChatId == chatId);
    }

    public async Task<IQueryable<Models.UserJoinRequest>> GetByUserId(long userId)
    {
        return (await All()).Where(x =>
            x.UserId == userId);
    }

    public async Task<Models.UserJoinRequest?> DeleteActive(long userId)
    {
        var target = await GetActive(userId);
        if (target != null)
            return await Delete(target);
        return null;
    }
}