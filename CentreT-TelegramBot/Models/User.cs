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

    public UserStatus Status { get; set; } = UserStatus.Default;

    public UserState State { get; set; } = UserState.Entry;

    public string? Name { get; set; } = null;

    public short? Age { get; set; } = null;

    public string? Pronouns { get; set; } = null;

    public string? Location { get; set; } = null;

    public User(long id)
    {
        Id = id;
    }

    public bool IsCompleted => Name != null && Age != null && Pronouns != null && Location != null;
}