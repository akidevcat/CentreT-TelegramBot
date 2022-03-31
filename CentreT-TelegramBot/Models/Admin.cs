using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CentreT_TelegramBot.Models.States;
using Microsoft.EntityFrameworkCore;

namespace CentreT_TelegramBot.Models;

[Index(nameof(UserId), IsUnique = true)]
public class Admin
{
    [ForeignKey(nameof(User))]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long UserId { get; set; }

    public User User { get; set; } = null!;
    
    public AdminState State { get; set; } = AdminState.Start;
}