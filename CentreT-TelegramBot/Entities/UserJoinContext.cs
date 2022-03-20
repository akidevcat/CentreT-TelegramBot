using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CentreT_TelegramBot.Entities.States;
using Microsoft.EntityFrameworkCore;

namespace CentreT_TelegramBot.Entities;

[Index(nameof(Id), nameof(UserId), IsUnique = true)]
public class UserJoinContext
{
    [Key]
    //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public Guid Id { get; set; }
    
    [ForeignKey(nameof(User))]
    public long UserId { get; set; }
    public User? User { get; set; }
    
    [ForeignKey(nameof(Chat))]
    public long ChatId { get; set; }
    public Chat? Chat { get; set; }
    
    public string? Name { get; set; }
    public string? Pronouns { get; set; }

    public UserJoinContextState State { get; set; } = UserJoinContextState.Chat;

    public UserJoinContext(long userId)
    {
        UserId = userId;
    }
}