using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CentreT_TelegramBot.Entities.States;

namespace CentreT_TelegramBot.Entities;

public class UserJoinContext
{
    [Key]
    public Guid Id { get; set; }
    
    [ForeignKey(nameof(User))]
    public long UserId { get; set; }
    public User? User { get; set; }
    
    [ForeignKey(nameof(Chat))]
    public long ChatId { get; set; }
    public Chat? Chat { get; set; }
    
    public string? Name { get; set; }
    public string? Pronouns { get; set; }
    
    public UserJoinContextState State { get; set; }
}