using System.ComponentModel.DataAnnotations.Schema;

namespace CentreT_TelegramBot.Models;

public abstract class UserRestriction
{
    [ForeignKey(nameof(User))]
    public long UserId { get; set; }
    public User? User { get; set; }
    
    [ForeignKey(nameof(Chat))]
    public long? ChatId { get; set; }
    public Chat? Chat { get; set; }
    
    public DateTime? ValidUntil { get; set; }
    
    public string? Reason { get; set; }
    
    [ForeignKey(nameof(IssuedByUser))]
    public long? IssuedByUserId { get; set; }
    public User? IssuedByUser { get; set; }
}