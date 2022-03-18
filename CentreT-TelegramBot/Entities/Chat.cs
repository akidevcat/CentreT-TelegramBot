using System.ComponentModel.DataAnnotations;

namespace CentreT_TelegramBot.Entities;

public class Chat
{
    [Key]
    public long Id { get; set; }
    public string? Name { get; set; }
}