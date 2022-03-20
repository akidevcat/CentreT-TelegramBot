using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CentreT_TelegramBot.Entities;

[Index(nameof(Id))]
public class User
{
    [Key]
    public long Id { get; set; }
}