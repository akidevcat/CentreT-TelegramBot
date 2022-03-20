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
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long UserId { get; set; }
    public User? User { get; set; }
    public UserContextState State { get; set; } = UserContextState.Start;

    public UserContext(long userId)
    {
        UserId = userId;
    }
}