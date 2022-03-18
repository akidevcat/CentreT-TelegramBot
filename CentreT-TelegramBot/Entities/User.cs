using System.ComponentModel.DataAnnotations;

namespace CentreT_TelegramBot.Entities;

public class User
{
    [Key]
    public long Id { get; set; }
}