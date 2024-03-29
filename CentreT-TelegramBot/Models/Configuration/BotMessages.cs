﻿using System.Diagnostics.CodeAnalysis;

namespace CentreT_TelegramBot.Models.Configuration;

/// <summary>
/// https://csharp2json.io/ Can be used for automatic conversion
/// </summary>
[Attributes.ConfigurationFile]
[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnassignedField.Global")]
public class BotMessages
{
    // State messages
    public string EntryMessage = "FE53E73B-AEFA-44D6-8B1B-5EB2A3175BDA";
    public string StartMessage = "BC951066-53A3-4B0E-BBE0-4A0E09DAB385";
    public string ProfileMessage = "6BD2BE7F-2857-456E-85A2-20297A148822";
    public string ProfileGetNameMessage = "75DE271E-D852-4B41-B4AE-D685488E1CAB";
    public string ProfileGetPronounsMessage = "82BDBA01-0B34-43F3-9E26-A9BD90C3F30D";
    public string ProfileGetAgeMessage = "0ECF9D00-BCC9-4082-92C4-165599D91CD9";
    public string ProfileGetLocationMessage = "4A8C530E-B628-4CE2-B4F1-EE327D7B8D6D";
    public string JoinConfirmationMessage = "503870EC-C32E-4B04-BB0A-1FD66AB9CD0D";
    // Action messages
    public string InformationMessage = "596093EF-710C-4246-BAEC-723633B76C42";
    public string JoinRequestCreated = "78751A1E-7163-47C6-B193-42513B087CC6";
    public string JoinRequestSent = "0B052252-9EF5-4A65-AAE4-981C3E454ED5";
    public string JoinRequestCancelled = "4A532D04-47D1-4D3F-836C-60581461420D";
    // Error messages
    public string InvalidAction = "F9396515-5C15-4FD2-973A-32FA263A174B";
    public string InvalidAge = "D416C91B-7765-4D5F-980A-45BE4DC52227";
    public string MessageTooLong = "7FF82DCC-8B8A-47E6-9F57-62480F30F9C3";
    public string ChatNotFound = "723E719B-3DDA-4DE0-A1E5-5B83B9D875B5";
    public string JoinRequestAlreadyExists = "47748A2D-66D6-4237-870F-E3E4610AA132";
    public string JoinRequestLimitReached = "20CB3943-758C-4882-AE51-F3EA9186E048";
    public string ShouldNotBeEmpty = "D821DF94-25D4-4E2C-BD71-A1BE8500B283";
    public string NotEnoughPermissions = "62C0E3A8-7266-46D0-BD99-D74689CF5439";
    // Admin messages
    public string ChatRegistrationCompleted = "32051CCC-5E7A-4323-A8A4-1EB081136290";
    public string ChatRegistrationFailed = "7E748719-4C44-4F7D-A401-B3DBAB004D4E";
    public string ChatUnregistrationCompleted = "C0F75718-66C1-43DC-86B0-C2108614F6BC";
    public string ChatUnregistrationFailed = "EFF57D66-988F-4C92-B1B8-4E7F1C1C50EE";
}