namespace NetCord;

/// <summary>
/// The type of a channel, dictating its features.
/// </summary>
public enum ChannelType
{
    /// <summary>
    /// A text channel within a guild.
    /// </summary>
    TextGuildChannel = 0,

    /// <summary>
    /// A text channel for direct messages between two users.
    /// </summary>
    DMChannel = 1,

    /// <summary>
    /// A voice channel within a guild.
    /// </summary>
    VoiceGuildChannel = 2,

    /// <summary>
    /// A text channel for direct messages between multiple users.
    /// </summary>
    GroupDMChannel = 3,

    /// <summary>
    /// An organizational category that contains up to 50 channels.
    /// </summary>
    CategoryChannel = 4,

    /// <summary>
    /// A channel that users can follow and crosspost into their own guild (formerly news channels).
    /// </summary>
    AnnouncementGuildChannel = 5,

    /// <summary>
    /// A channel that allows users to purchase subscriptions, roles or downloadables for real-life currency in guilds.
    /// </summary>
    StoreGuildChannel = 6,

    /// <summary>
    /// A temporary sub-channel within an <see cref="AnnouncementGuildChannel"/>.
    /// </summary>
    AnnouncementGuildThread = 10,

    /// <summary>
    /// A temporary sub-channel within a <see cref="TextGuildChannel"/> or <see cref="ForumGuildChannel"/>.
    /// </summary>
    PublicGuildThread = 11,

    /// <summary>
    /// A temporary sub-channel within a <see cref="TextGuildChannel"/> that is only viewable by those invited and/or those with the <see cref="Permissions.ManageThreads"/> permission.
    /// </summary>
    PrivateGuildThread = 12,

    /// <summary>
    /// A voice channel for hosting events with an audience.
    /// </summary>
    StageGuildChannel = 13,

    /// <summary>
    /// The channel in a hub containing the listed guilds.
    /// </summary>
    DirectoryGuildChannel = 14,

    /// <summary>
    /// A channel that can only contain threads.
    /// </summary>
    ForumGuildChannel = 15,

    /// <summary>
    /// Channels that can only contain threads, similar to a <see cref="ForumGuildChannel"/>, but still in active development.
    /// </summary>
    MediaForumGuildChannel = 16,
}
