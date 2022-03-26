namespace CentreT_TelegramBot.Entities.States;

public enum UserEvent
{
    StartCommand,
    InformationCommand, JoinCommand, ProfileCommand,
    BackCommand, NextCommand, ConfirmCommand,
    UpdateMessage,
    ArgumentFilled
}