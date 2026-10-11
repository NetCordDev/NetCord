namespace NetCord;

/// <summary>
/// Represents additional information about a channel's state.
/// </summary>
[Flags]
public enum ChannelFlags
{
    /// <summary>
    /// Channel is excluded from the guild feed.
    /// </summary>
    GuildFeedRemoved = 1 << 0,

    /// <summary>
    /// Whether a thread is pinned to the top of its parent <see cref="ForumGuildChannel"/> or <see cref="MediaForumGuildChannel"/>.
    /// </summary>
    Pinned = 1 << 1,

    /// <summary>
    /// Channel is excluded from the guild's active channels.
    /// </summary>
    ActiveChannelsRemoved = 1 << 2,

    /// <summary>
    /// Whether a tag is required to be specified when creating a thread in a <see cref="ForumGuildChannel"/> or <see cref="MediaForumGuildChannel"/>.
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
    /// Channel is exempt from AI-powered summaries.
    /// </summary>
    SummariesDisabled = 1 << 11,

    /// <summary>
    /// Private channel's recipients consented to the application shelf.
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
    /// Hides the embedded media download options. Available only for media channels.
    /// </summary>
    HideMediaDownloadOptions = 1 << 15,

    /// <summary>
    /// This channel's metadata has been obfuscated because the current user cannot view it. Only ever set on channels received over the Gateway; the HTTP API never sets this flag. See Obfuscated Channels.
    /// </summary>
    ChannelObfuscated = 1 << 17,

    /// <summary>
    /// This channel is a Spoiler Channel i.e. users must opt in to view its contents. Can be set on all textual guild channels and voice channels (not <see cref="ChannelType.StageGuildChannel"/>). Can only be set if channel's nsfw is false.
    /// </summary>
    IsSpoilerChannel = 1 << 21,
}
