using System.ComponentModel.DataAnnotations;

namespace CentreT_TelegramBot.Entities;

public class UserBan : UserRestriction
{
    [Key]
    public Guid Id { get; set; }
}