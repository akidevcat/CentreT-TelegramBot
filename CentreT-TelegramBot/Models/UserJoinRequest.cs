using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CentreT_TelegramBot.Entities;

namespace CentreT_TelegramBot.Models;

public class UserJoinRequest
{
    [Key]
    public Guid Id { get; set; }
    
    [ForeignKey(nameof(User))]
    public long UserId { get; set; }
    public User? User { get; set; }
    
    [ForeignKey(nameof(Chat))]
    public long? ChatId { get; set; } = null;
    public Chat? Chat { get; set; }

    public DateTime? DateCreated { get; set; } = null;
    
    public UserJoinRequest(long userId)
    {
        UserId = userId;
    }

    public void Complete()
    {
        DateCreated = DateTime.Now;
    }

    public bool Completed => ChatId != null;

    public bool Active => DateCreated == null;
}