using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CentreT_TelegramBot.Entities.States;
using Microsoft.EntityFrameworkCore;

namespace CentreT_TelegramBot.Entities;

[Index(nameof(UserId))]
public class UserContext
{
    [Key]
    [ForeignKey(nameof(User))]
    public long UserId { get; set; }
    public User? User { get; set; }
    public States.UserContextState State { get; set; }

    public UserContext(long userId)
    {
        UserId = userId;
        State = UserContextState.Start;
    }
}