using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CentreT_TelegramBot.Entities;

[Index(nameof(Id), IsUnique = true)]
[Index(nameof(Name), IsUnique = false)]
public class Chat
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; set; }
    [MaxLength(30)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Name { get; set; } = null!;
}