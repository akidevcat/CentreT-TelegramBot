using CentreT_TelegramBot.Entities;

namespace CentreT_TelegramBot.Services;

public interface IRepositoryService
{
    /// <summary>
    /// Gets <see cref="UserContext"/>. If does not exist, creates a new default <see cref="UserContext"/>.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <returns><see cref="UserContext"/> found or created.</returns>
    Task<UserContext> GetUserContext(long userId);

    /// <summary>
    /// Gets <see cref="User"/>. If does not exist, creates a new default <see cref="User"/>.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <returns><see cref="User"/> found or created.</returns>
    Task<User> GetUser(long userId);
}