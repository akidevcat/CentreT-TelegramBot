namespace CentreT_TelegramBot.Models.States;

public enum UserEvent
{
    StartCommand,
    InformationCommand, JoinCommand, ProfileCommand, EditCommand,
    BackCommand, NextCommand, ConfirmCommand,
    UpdateMessage,
    ArgumentFilled
}