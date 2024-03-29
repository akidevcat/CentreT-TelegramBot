﻿using CentreT_TelegramBot.Repositories.Infrastructure;

namespace CentreT_TelegramBot.Repositories;

public interface IUserRepository : IGenericRepository<Models.User>
{
    Task<Models.User> GetOrCreate(long userId);
}