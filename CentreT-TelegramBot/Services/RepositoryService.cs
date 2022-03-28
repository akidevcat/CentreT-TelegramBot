using CentreT_TelegramBot.Models;
using CentreT_TelegramBot.Repositories;

namespace CentreT_TelegramBot.Services;

public class RepositoryService : IRepositoryService
{

    private readonly IUserRepository _userRepository;
    private readonly IUserJoinRequestRepository _userJoinRequestRepository;
    private readonly IChatRepository _chatRepository;
    
    public RepositoryService(IUserRepository userRepository, IUserJoinRequestRepository userJoinRequestRepository
        , IChatRepository chatRepository)
    {
        _userRepository = userRepository;
        _userJoinRequestRepository = userJoinRequestRepository;
        _chatRepository = chatRepository;
    }

    public async Task<Chat?> GetChat(string name)
    {
        return await _chatRepository.GetFirst(x =>
            x.Name == name);
    }

    public async Task<IQueryable<Chat>> GetAllChats()
    {
        return await _chatRepository.All();
    }

    public async Task<UserJoinRequest?> GetActiveUserJoinRequest(long userId)
    {
        return await _userJoinRequestRepository.GetFirst(x =>
            x.UserId == userId && x.DateCreated == null);
    }

    public async Task<UserJoinRequest?> GetUserJoinRequestByChat(long chatId)
    {
        return await _userJoinRequestRepository.GetFirst(x =>
            x.ChatId == chatId);
    }

    public async Task<UserJoinRequest?> GetUserJoinRequestByUser(long userId)
    {
        return await _userJoinRequestRepository.GetFirst(x =>
            x.UserId == userId);
    }

    public async Task<IQueryable<UserJoinRequest>> GetUserJoinRequestsByUser(long userId)
    {
        return await _userJoinRequestRepository.Get(x =>
            x.UserId == userId && x.DateCreated == null);
    }

    public async Task<UserJoinRequest> CreateUserJoinRequest(long userId)
    {
        return await _userJoinRequestRepository.Create(new UserJoinRequest(userId));
    }

    public async Task<UserJoinRequest> GetOrCreateActiveUserJoinRequest(long userId)
    {
        var request = await GetActiveUserJoinRequest(userId);
        if (request != null)
            return request;
        
        return await CreateUserJoinRequest(userId);
    }

    // public async Task<UserJoinContext?> GetActiveUserJoinContext(long userId)
    // {
    //     return await _userJoinContextRepository.Get(x =>
    //         x.UserId == userId && x.State != UserJoinContextState.AwaitingResponse);
    // }
    //
    // public async Task<UserJoinContext> GetOrCreateActiveUserJoinContext(long userId)
    // {
    //     // Create new User if it does not exist
    //     await GetOrCreateUser(userId);
    //     // Get or create UserJoinContext
    //     // UserJoinContext which are AwaitingResponse should be ignored as they do not lock the state
    //     return await _userJoinContextRepository.GetOrCreate(new UserJoinContext(userId), 
    //         x => x.UserId == userId && x.State != UserJoinContextState.AwaitingResponse);
    // }
    //
    // public async Task<UserContext> GetOrCreateUserContext(long userId)
    // {
    //     // Create new User if it does not exist
    //     await GetOrCreateUser(userId);
    //     // Get or create UserContext
    //     return await _userContextRepository.GetOrCreate(new UserContext(userId), true, userId);
    // }
    //
    // public Task<UserContext> UpdateUserContext(UserContext userContext)
    // {
    //     throw new NotImplementedException();
    // }

    public async Task<User> GetOrCreateUser(long userId)
    {
        // Get or create User
        return await _userRepository.GetOrCreate(new User(userId), true, userId);
    }

    public async Task<bool> DeleteActiveUserJoinRequest(long userId)
    {
        var result = await GetActiveUserJoinRequest(userId);
        if (result != null)
        {
            await _userJoinRequestRepository.Delete(result);
            return true;
        }

        return false;
    }

    // public async Task DeleteUserJoinContext(UserJoinContext userJoinContext)
    // {
    //     await _userJoinContextRepository.Delete(userJoinContext);
    // }

    public async Task SaveChanges()
    {
        await _userRepository.Save();
        await _userJoinRequestRepository.Save();
        // await _userContextRepository.Save();
    }
}