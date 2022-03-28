using CentreT_TelegramBot.Models;

namespace CentreT_TelegramBot.Services;

public interface IRepositoryService
{
    Task<Chat?> GetChat(string name);

    Task<IQueryable<Chat>> GetAllChats();

    Task<UserJoinRequest?> GetActiveUserJoinRequest(long userId);
    
    Task<UserJoinRequest?> GetUserJoinRequestByChat(long chatId);
    
    Task<UserJoinRequest?> GetUserJoinRequestByUser(long userId);
    
    Task<IQueryable<UserJoinRequest>> GetUserJoinRequestsByUser(long userId);
    
    Task<UserJoinRequest> CreateUserJoinRequest(long userId);
    
    Task<UserJoinRequest> GetOrCreateActiveUserJoinRequest(long userId);

    /// <summary>
    /// Gets <see cref="User"/>. If does not exist, creates a new default <see cref="User"/>.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <returns><see cref="User"/> found or created.</returns>
    Task<User> GetOrCreateUser(long userId);

    Task<bool> DeleteActiveUserJoinRequest(long userId);
    
    /// <summary>
    /// Saves any changes implicitly made.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task SaveChanges();
}