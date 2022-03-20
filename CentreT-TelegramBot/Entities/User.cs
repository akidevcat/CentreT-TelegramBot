using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CentreT_TelegramBot.Entities.States;
using Microsoft.EntityFrameworkCore;

namespace CentreT_TelegramBot.Entities;

[Index(nameof(Id), IsUnique = true)]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; set; }

    public UserStatus UserStatus { get; set; } = UserStatus.Default;

    public UserContext UserContext { get; set; }

    public User(long id)
    {
        Id = id;
    }
}