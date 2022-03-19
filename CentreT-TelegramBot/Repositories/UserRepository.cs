﻿using CentreT_TelegramBot.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CentreT_TelegramBot.Repositories;

public class UserRepository : GenericRepository<Entities.User>, IUserRepository
{
    public UserRepository(BotDbContext dbContext) : base(dbContext)
    {
        
    }
}