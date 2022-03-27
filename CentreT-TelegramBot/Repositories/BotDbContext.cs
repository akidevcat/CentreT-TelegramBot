// ReSharper disable UnusedMember.Global
#pragma warning disable CS8618

using CentreT_TelegramBot.Models;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;

namespace CentreT_TelegramBot.Repositories;

public class BotDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<UserJoinRequest> UserJoinRequests { get; set; }

    public BotDbContext(DbContextOptions<BotDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Chat

        modelBuilder.Entity<Chat>()
            .HasAlternateKey(nameof(Chat.Name));

        #endregion
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseExceptionProcessor();
    }
}