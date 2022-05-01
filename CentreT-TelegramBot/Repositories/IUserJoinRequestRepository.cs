using CentreT_TelegramBot.Repositories.Infrastructure;

namespace CentreT_TelegramBot.Repositories;

public interface IUserJoinRequestRepository : IGenericRepository<Models.UserJoinRequest>
{
    Task<Models.UserJoinRequest?> CreateActive(long userId);
    
    Task<Models.UserJoinRequest?> GetActive(long userId);

    Task<Models.UserJoinRequest?> Get(long userId, long chatId);
    
    Task<IQueryable<Models.UserJoinRequest>> GetByUserId(long userId);

    Task<Models.UserJoinRequest?> DeleteActive(long userId);
}