using CentreT_TelegramBot.Entities;
using CentreT_TelegramBot.Repositories;

namespace CentreT_TelegramBot.Services;

public class RepositoryService : IRepositoryService
{

    private readonly IUserRepository _userRepository;
    private readonly IUserContextRepository _userContextRepository;
    private readonly IUserJoinContextRepository _userJoinContextRepository;
    
    public RepositoryService(IUserRepository userRepository, IUserContextRepository userContextRepository, 
                             IUserJoinContextRepository userJoinContextRepository)
    {
        _userRepository = userRepository;
        _userContextRepository = userContextRepository;
        _userJoinContextRepository = userJoinContextRepository;
    }

    public async Task<UserJoinContext> GetUserJoinContext(long userId)
    {
        // Create new User if it does not exist
        await GetUser(userId);
        // Get or create UserJoinContext
        return await _userJoinContextRepository.GetOrCreate(new UserJoinContext(userId), x => x.UserId == userId);
    }

    public async Task<UserContext> GetUserContext(long userId)
    {
        // Create new User if it does not exist
        await GetUser(userId);
        // Get or create UserContext
        return await _userContextRepository.GetOrCreate(new UserContext(userId), true, userId);
    }

    public Task<UserContext> UpdateUserContext(UserContext userContext)
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetUser(long userId)
    {
        // Get or create User
        return await _userRepository.GetOrCreate(new User(userId), true, userId);
    }

    public async Task SaveChanges()
    {
        await _userRepository.Save();
        await _userContextRepository.Save();
    }
}