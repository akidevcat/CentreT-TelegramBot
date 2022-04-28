using CentreT_TelegramBot.Repositories.Infrastructure;

namespace CentreT_TelegramBot.Repositories;

public interface IChatRepository : IGenericRepository<Models.Chat>
{
    Task<Models.Chat?> GetChat(long id);
    
    Task<Models.Chat?> GetChat(string name);

    Task<Models.Chat?> CreateChat(Models.Chat chat);

    Task<bool> DeleteChat(Models.Chat chat);
    
    Task<bool> DeleteChat(long id);

    Task<IQueryable<Models.Chat>> GetAllChats();
}