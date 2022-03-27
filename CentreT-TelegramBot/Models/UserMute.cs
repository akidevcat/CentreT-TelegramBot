using System.ComponentModel.DataAnnotations;

namespace CentreT_TelegramBot.Models;

public class UserMute : UserRestriction
{
    [Key]
    public Guid Id { get; set; }
}