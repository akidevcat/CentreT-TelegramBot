using CentreT_TelegramBot.Entities;
using CentreT_TelegramBot.Entities.States;
using CentreT_TelegramBot.Models;

namespace CentreT_TelegramBot.Services;

public interface IRepositoryService
{
    Task<Chat?> GetChat(string name);

    Task<UserJoinRequest?> GetActiveUserJoinRequest(long userId);
    
    Task<UserJoinRequest?> GetUserJoinRequestByChat(long chatId);
    
    Task<UserJoinRequest?> GetUserJoinRequestByUser(long userId);
    
    Task<IQueryable<UserJoinRequest>> GetUserJoinRequestsByUser(long userId);
    
    Task<UserJoinRequest> CreateUserJoinRequest(long userId);

    // /// <summary>
    // /// Gets an active (not <see cref="UserJoinContextState.AwaitingResponse"/> state) <see cref="UserJoinContext"/>.
    // /// </summary>
    // /// <param name="userId">User ID.</param>
    // /// <returns>Found <see cref="UserJoinContext"/>, or null.</returns>
    // Task<UserJoinContext?> GetActiveUserJoinContext(long userId);
    //
    // /// <summary>
    // /// Gets an active (not <see cref="UserJoinContextState.AwaitingResponse"/> state) <see cref="UserJoinContext"/>.
    // /// If does not exist, creates a new active <see cref="UserJoinContext"/>.
    // /// </summary>
    // /// <param name="userId">User ID.</param>
    // /// <returns><see cref="UserJoinContext"/> found or created.</returns>
    // Task<UserJoinContext> GetOrCreateActiveUserJoinContext(long userId);
    
    // /// <summary>
    // /// Gets <see cref="UserContext"/>. If does not exist, creates a new default <see cref="UserContext"/>.
    // /// </summary>
    // /// <param name="userId">User ID.</param>
    // /// <returns><see cref="UserContext"/> found or created.</returns>
    // Task<UserContext> GetOrCreateUserContext(long userId);

    // /// <summary>
    // /// Updates <see cref="UserContext"/>. If does not exist, creates <see cref="UserContext"/>.
    // /// </summary>
    // /// <param name="userContext"><see cref="UserContext"/>.</param>
    // /// <returns>Updated <see cref="UserContext"/>.</returns>
    // Task<UserContext> UpdateUserContext(UserContext userContext);

    /// <summary>
    /// Gets <see cref="User"/>. If does not exist, creates a new default <see cref="User"/>.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <returns><see cref="User"/> found or created.</returns>
    Task<User> GetOrCreateUser(long userId);

    //Task DeleteUserJoinContext(UserJoinContext userJoinContext);
    Task DeleteActiveUserJoinRequest(long userId);
    
    /// <summary>
    /// Saves any changes implicitly made.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task SaveChanges();
}