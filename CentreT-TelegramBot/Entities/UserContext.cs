using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentreT_TelegramBot.Entities;

public class UserContext
{
    [ForeignKey(nameof(User))]
    public long UserId { get; set; }
    public User? User { get; set; }
    public States.UserContextState State { get; set; }
}