using System.ComponentModel.DataAnnotations;

namespace CentreT_TelegramBot.Entities;

public class UserMute : UserRestriction
{
    [Key]
    public Guid Id { get; set; }
}