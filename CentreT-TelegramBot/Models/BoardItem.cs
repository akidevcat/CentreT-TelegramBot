namespace CentreT_TelegramBot.Models;

[Flags]
public enum BoardItemTags
{
    RealEstate, Apartments, Rent
}

public class BoardItem
{
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public BoardItemTags Flags { get; set; }
}