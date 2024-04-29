namespace NetCord;

[Flags]
public enum ChannelFlags
{
    /// <summary>
    /// Channel is excluded from the guild feed.
    /// </summary>
    GuildFeedRemoved = 1 << 0,

    /// <summary>
    /// Post is pinned to its forum channel.
    /// </summary>
    Pinned = 1 << 1,

    /// <summary>
    /// Channel is excluded from the guild's active channels.
    /// </summary>
    ActiveChannelsRemoved = 1 << 2,

    /// <summary>
    /// Forum requires a tag for posts.
    /// </summary>
    RequireTag = 1 << 4,

    /// <summary>
    /// Channel is marked as spam.
    /// </summary>
    Spam = 1 << 5,

    /// <summary>
    /// Channel is a hidden read-only resource channel for use during onboarding.
    /// </summary>
    GuildResourceChannel = 1 << 7,

    /// <summary>
    /// Thread was created by Clyde.
    /// </summary>
    ClydeAI = 1 << 8,

    /// <summary>
    /// Channel is scheduled for deletion and hidden.
    /// </summary>
    ScheduledForDeletion = 1 << 9,

    /// <summary>
    /// Channel is a media channel.
    /// </summary>
    MediaChannel = 1 << 10,

    /// <summary>
    /// Channel is exempt from 'For You' summaries.
    /// </summary>
    SummariesDisabled = 1 << 11,

    /// <summary>
    /// 
    /// </summary>
    ApplicationShelfConsent = 1 << 12,

    /// <summary>
    /// Channel is a role subscription template preview.
    /// </summary>
    RoleSubscriptionTemplatePreviewChannel = 1 << 13,

    /// <summary>
    /// Channel has a global broadcast.
    /// </summary>
    Broadcasting = 1 << 14,

    /// <summary>
    /// Channel has no download options for media, media channel exclusive.
    /// </summary>
    HideMediaDownloadOptions = 1 << 15,
}
