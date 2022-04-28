using CentreT_TelegramBot.Models;
using CentreT_TelegramBot.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CentreT_TelegramBot.Repositories;

public class ChatRepository : GenericRepository<Models.Chat>, IChatRepository
{
    public ChatRepository(BotDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Chat?> GetChat(long id)
    {
        return await GetFirst(x =>
            x.Id == id);
    }
    
    public async Task<Chat?> GetChat(string name)
    {
        return await GetFirst(x =>
            x.Name == name);
    }

    public async Task<Chat?> CreateChat(Chat chat)
    {
        var result = await GetChat(chat.Id);
        if (result == null)
        {
            return await Create(chat);
        }

        return null;
    }

    public async Task<bool> DeleteChat(Chat chat)
    {
        var result = await GetChat(chat.Id);
        if (result != null)
        {
            await Delete(result);
            return true;
        }

        return false;
    }
    
    public async Task<bool> DeleteChat(long id)
    {
        var result = await GetChat(id);
        if (result != null)
        {
            await Delete(result);
            return true;
        }

        return false;
    }

    public async Task<IQueryable<Chat>> GetAllChats()
    {
        return await All();
    }
}