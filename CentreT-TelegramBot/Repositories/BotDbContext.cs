// ReSharper disable UnusedMember.Global
#pragma warning disable CS8618

using CentreT_TelegramBot.Entities;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;

namespace CentreT_TelegramBot.Repositories;

public class BotDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserContext> UserContexts { get; set; }
    public DbSet<UserJoinContext> UserJoinContexts { get; set; }

    public BotDbContext(DbContextOptions<BotDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Chat

        // modelBuilder.Entity<Chat>()
        //     .HasKey(nameof(Chat.Id), nameof(Chat.Name));
        modelBuilder.Entity<Chat>()
            .HasAlternateKey(nameof(Chat.Name));

        #endregion
        
        #region UserJoinContext

        // modelBuilder.Entity<UserJoinContext>()
        //     .HasKey(nameof(UserJoinContext.Id), nameof(UserJoinContext.UserId));
        modelBuilder.Entity<UserJoinContext>()
            .HasAlternateKey(nameof(UserJoinContext.UserId));
        
        modelBuilder.Entity<UserJoinContext>()
            .HasOne(nameof(UserJoinContext.Chat))
            .WithOne()
            .HasPrincipalKey(typeof(Chat), nameof(Chat.Name));

        #endregion


    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseExceptionProcessor();
    }
}