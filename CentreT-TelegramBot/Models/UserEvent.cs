namespace CentreT_TelegramBot.Models.States;

public enum UserEvent
{
    // Base user events, 0 - 99
    StartCommand = 0,
    InformationCommand = 1, 
    JoinCommand = 2, 
    ProfileCommand = 3, 
    EditCommand = 4,
    BackCommand = 5, 
    NextCommand = 6, 
    ConfirmCommand = 7,
    ArgumentFilled = 8,
    // Moderator events 100 - 199
    BanCommand = 100,
    UnbanCommand = 101,
    MuteCommand = 102,
    UnmuteCommand = 103,
    // Admin events 200 - 299
    RegisterCommand = 200,
    UnregisterCommand = 201
}