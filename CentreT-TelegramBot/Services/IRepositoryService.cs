using CentreT_TelegramBot.Entities;

namespace CentreT_TelegramBot.Services;

public interface IRepositoryService
{
    /// <summary>
    /// Gets <see cref="UserJoinContext"/>. If does not exist, creates a new default <see cref="UserJoinContext"/>.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <returns><see cref="UserJoinContext"/> found or created.</returns>
    Task<UserJoinContext> GetUserJoinContext(long userId);
    
    /// <summary>
    /// Gets <see cref="UserContext"/>. If does not exist, creates a new default <see cref="UserContext"/>.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <returns><see cref="UserContext"/> found or created.</returns>
    Task<UserContext> GetUserContext(long userId);

    /// <summary>
    /// Updates <see cref="UserContext"/>. If does not exist, creates <see cref="UserContext"/>.
    /// </summary>
    /// <param name="userContext"><see cref="UserContext"/>.</param>
    /// <returns>Updated <see cref="UserContext"/>.</returns>
    Task<UserContext> UpdateUserContext(UserContext userContext);

    /// <summary>
    /// Gets <see cref="User"/>. If does not exist, creates a new default <see cref="User"/>.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <returns><see cref="User"/> found or created.</returns>
    Task<User> GetUser(long userId);

    /// <summary>
    /// Saves any changes implicitly made.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task SaveChanges();
}