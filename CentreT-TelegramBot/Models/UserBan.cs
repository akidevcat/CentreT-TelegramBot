using System.ComponentModel.DataAnnotations;

namespace CentreT_TelegramBot.Models;

public class UserBan : UserRestriction
{
    [Key]
    public Guid Id { get; set; }
}