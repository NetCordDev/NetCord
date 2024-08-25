﻿namespace NetCord;

public enum AuditLogEvent
{
    /// <summary>
    /// Server settings were updated.
    /// </summary>
    GuildUpdate = 1,

    /// <summary>
    /// Channel was created.
    /// </summary>
    ChannelCreate = 10,

    /// <summary>
    /// Channel settings were updated.
    /// </summary>
    ChannelUpdate = 11,

    /// <summary>
    /// Channel was deleted.
    /// </summary>
    ChannelDelete = 12,

    /// <summary>
    /// Permission overwrite was added to a channel.
    /// </summary>
    ChannelOverwriteCreate = 13,

    /// <summary>
    /// Permission overwrite was updated for a channel.
    /// </summary>
    ChannelOverwriteUpdate = 14,

    /// <summary>
    /// Permission overwrite was deleted from a channel.
    /// </summary>
    ChannelOverwriteDelete = 15,

    /// <summary>
    /// Guild user was removed from server.
    /// </summary>
    GuildUserKick = 20,

    /// <summary>
    /// GuildUsers were pruned from server.
    /// </summary>
    GuildUserPrune = 21,

    /// <summary>
    /// Guild user was banned from server.
    /// </summary>
    GuildUserBanAdd = 22,

    /// <summary>
    /// Server ban was lifted for a guild user.
    /// </summary>
    GuildUserBanRemove = 23,

    /// <summary>
    /// Guild user was updated in server.
    /// </summary>
    GuildUserUpdate = 24,

    /// <summary>
    /// Guild user was added or removed from a role.
    /// </summary>
    GuildUserRoleUpdate = 25,

    /// <summary>
    /// Guild user was moved to a different voice channel.
    /// </summary>
    GuildUserMove = 26,

    /// <summary>
    /// Guild user was disconnected from a voice channel.
    /// </summary>
    GuildUserDisconnect = 27,

    /// <summary>
    /// Bot user was added to server.
    /// </summary>
    BotAdd = 28,

    /// <summary>
    /// Role was created.
    /// </summary>
    RoleCreate = 30,

    /// <summary>
    /// Role was edited.
    /// </summary>
    RoleUpdate = 31,

    /// <summary>
    /// Role was deleted.
    /// </summary>
    RoleDelete = 32,

    /// <summary>
    /// Server invite was created.
    /// </summary>
    InviteCreate = 40,

    /// <summary>
    /// Server invite was updated.
    /// </summary>
    InviteUpdate = 41,

    /// <summary>
    /// Server invite was deleted.
    /// </summary>
    InviteDelete = 42,

    /// <summary>
    /// Webhook was created.
    /// </summary>
    WebhookCreate = 50,

    /// <summary>
    /// Webhook properties or channel were updated.
    /// </summary>
    WebhookUpdate = 51,

    /// <summary>
    /// Webhook was deleted.
    /// </summary>
    WebhookDelete = 52,

    /// <summary>
    /// Emoji was created.
    /// </summary>
    EmojiCreate = 60,

    /// <summary>
    /// Emoji name was updated.
    /// </summary>
    EmojiUpdate = 61,

    /// <summary>
    /// Emoji was deleted.
    /// </summary>
    EmojiDelete = 62,

    /// <summary>
    /// Single message was deleted.
    /// </summary>
    MessageDelete = 72,

    /// <summary>
    /// Multiple messages were deleted.
    /// </summary>
    MessageBulkDelete = 73,

    /// <summary>
    /// Message was pinned to a channel.
    /// </summary>
    MessagePin = 74,

    /// <summary>
    /// Message was unpinned from a channel.
    /// </summary>
    MessageUnpin = 75,

    /// <summary>
    /// App was added to server.
    /// </summary>
    IntegrationCreate = 80,

    /// <summary>
    /// App was updated (as an example, its scopes were updated).
    /// </summary>
    IntegrationUpdate = 81,

    /// <summary>
    /// App was removed from server.
    /// </summary>
    IntegrationDelete = 82,

    /// <summary>
    /// Stage instance was created (stage channel becomes live).
    /// </summary>
    StageInstanceCreate = 83,

    /// <summary>
    /// Stage instance details were updated.
    /// </summary>
    StageInstanceUpdate = 84,

    /// <summary>
    /// Stage instance was deleted (stage channel no longer live).
    /// </summary>
    StageInstanceDelete = 85,

    /// <summary>
    /// Sticker was created.
    /// </summary>
    StickerCreate = 90,

    /// <summary>
    /// Sticker details were updated.
    /// </summary>
    StickerUpdate = 91,

    /// <summary>
    /// Sticker was deleted.
    /// </summary>
    StickerDelete = 92,

    /// <summary>
    /// Event was created.
    /// </summary>
    GuildScheduledEventCreate = 100,

    /// <summary>
    /// Event was updated.
    /// </summary>
    GuildScheduledEventUpdate = 101,

    /// <summary>
    /// Event was cancelled.
    /// </summary>
    GuildScheduledEventDelete = 102,

    /// <summary>
    /// Thread was created in a channel.
    /// </summary>
    ThreadCreate = 110,

    /// <summary>
    /// Thread was updated.
    /// </summary>
    ThreadUpdate = 111,

    /// <summary>
    /// Thread was deleted.
    /// </summary>
    ThreadDelete = 112,

    /// <summary>
    /// Permissions were updated for a command.
    /// </summary>
    ApplicationCommandPermissionUpdate = 121,

    /// <summary>
    /// Auto Moderation rule was created.
    /// </summary>
    AutoModerationRuleCreate = 140,

    /// <summary>
    /// Auto Moderation rule was updated.
    /// </summary>
    AutoModerationRuleUpdate = 141,

    /// <summary>
    /// Auto Moderation rule was deleted.
    /// </summary>
    AutoModerationRuleDelete = 142,

    /// <summary>
    /// Message was blocked by Auto Moderation.
    /// </summary>
    AutoModerationBlockMessage = 143,

    /// <summary>
    /// Message was flagged by Auto Moderation.
    /// </summary>
    AutoModerationFlagToChannel = 144,

    /// <summary>
    /// User was timed out by Auto Moderation.
    /// </summary>
    AutoModerationUserCommunicationDisabled = 145,

    /// <summary>
    /// Creator monetization request was created.
    /// </summary>
    CreatorMonetizationRequestCreated = 150,

    /// <summary>
    /// Creator monetization terms were accepted.
    /// </summary>
    CreatorMonetizationTermsAccepted = 151,

    /// <summary>
    /// Guild onboarding question was created.
    /// </summary>
    OnboardingPromptCreate = 163,

    /// <summary>
    /// Guild onboarding question was updated.
    /// </summary>
    OnboardingPromptUpdate = 164,

    /// <summary>
    /// Guild onboarding question was deleted.
    /// </summary>
    OnboardingPromptDelete = 165,

    /// <summary>
    /// Guild onboarding was created.
    /// </summary>
    OnboardingCreate = 166,

    /// <summary>
    /// Guild onboarding was updated.
    /// </summary>
    OnboardingUpdate = 167,

    /// <summary>
    /// Server guide was created.
    /// </summary>
    HomeSettingsCreate = 190,

    /// <summary>
    /// Server guide was updated.
    /// </summary>
    HomeSettingsUpdate = 191,
}
