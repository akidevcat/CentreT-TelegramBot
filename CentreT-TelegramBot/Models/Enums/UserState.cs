namespace CentreT_TelegramBot.Models.States;

public enum UserState
{
    // Base user states 0 - 99
    Entry = 0, 
    Start = 1, 
    Profile = 2, 
    ProfileGetName = 3, 
    ProfileGetPronouns = 4, 
    ProfileGetAge = 5, 
    ProfileGetLocation = 6,
    JoinConfirmation = 7,
    // Moderator states 100 - 199
    // Admin states 200 - 299
}