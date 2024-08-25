namespace NetCord;

[Flags]
public enum Permissions : ulong
{
    /// <summary>
    /// Allows creation of instant invites.
    /// </summary>
    CreateInstantInvite = 1uL << 0,

    /// <summary>
    /// Allows kicking guild users.
    /// </summary>
    KickUsers = 1uL << 1,

    /// <summary>
    /// Allows banning guild users.
    /// </summary>
    BanUsers = 1uL << 2,

    /// <summary>
    /// Allows all permissions and bypasses channel permission overwrites.
    /// </summary>
    Administrator = 1uL << 3,

    /// <summary>
    /// Allows management and editing of channels.
    /// </summary>
    ManageChannels = 1uL << 4,

    /// <summary>
    /// Allows management and editing of the guild.
    /// </summary>
    ManageGuild = 1uL << 5,

    /// <summary>
    /// Allows for the addition of reactions to messages.
    /// </summary>
    AddReactions = 1uL << 6,

    /// <summary>
    /// Allows for viewing of audit logs.
    /// </summary>
    ViewAuditLog = 1uL << 7,

    /// <summary>
    /// Allows for using priority speaker in a voice channel.
    /// </summary>
    PrioritySpeaker = 1uL << 8,

    /// <summary>
    /// Allows the user to go live.
    /// </summary>
    Stream = 1uL << 9,

    /// <summary>
    /// Allows guild users to view a channel, which includes reading messages in text channels and joining voice channels.
    /// </summary>
    ViewChannel = 1uL << 10,

    /// <summary>
    /// Allows for sending messages in a channel and creating threads in a forum (does not allow sending messages in threads).
    /// </summary>
    SendMessages = 1uL << 11,

    /// <summary>
    /// Allows for sending of /tts messages.
    /// </summary>
    SendTtsMessages = 1uL << 12,

    /// <summary>
    /// Allows for deletion of other users messages.
    /// </summary>
    ManageMessages = 1uL << 13,

    /// <summary>
    /// Links sent by users with this permission will be auto-embedded.
    /// </summary>
    EmbedLinks = 1uL << 14,

    /// <summary>
    /// Allows for uploading images and files.
    /// </summary>
    AttachFiles = 1uL << 15,

    /// <summary>
    /// Allows for reading of message history.
    /// </summary>
    ReadMessageHistory = 1uL << 16,

    /// <summary>
    /// Allows for using the <see langword="@everyone"/> tag to notify all users in a channel, and the <see langword="@here"/> tag to notify all online users in a channel.
    /// </summary>
    MentionEveryone = 1uL << 17,

    /// <summary>
    /// Allows the usage of custom emojis from other servers.
    /// </summary>
    UseExternalEmojis = 1uL << 18,

    /// <summary>
    /// Allows for viewing guild insights.
    /// </summary>
    ViewGuildInsights = 1uL << 19,

    /// <summary>
    /// Allows for joining of a voice channel.
    /// </summary>
    Connect = 1uL << 20,

    /// <summary>
    /// Allows for speaking in a voice channel.
    /// </summary>
    Speak = 1uL << 21,

    /// <summary>
    /// Allows for muting users in a voice channel.
    /// </summary>
    MuteUsers = 1uL << 22,

    /// <summary>
    /// Allows for deafening of users in a voice channel.
    /// </summary>
    DeafenUsers = 1uL << 23,

    /// <summary>
    /// Allows for moving of users between voice channels.
    /// </summary>
    MoveUsers = 1uL << 24,

    /// <summary>
    /// Allows for using voice-activity-detection in a voice channel.
    /// </summary>
    UseVoiceActivityDetection = 1uL << 25,

    /// <summary>
    /// Allows for modification of own nickname.
    /// </summary>
    ChangeNickname = 1uL << 26,

    /// <summary>
    /// Allows for modification of other users nicknames.
    /// </summary>
    ManageNicknames = 1uL << 27,

    /// <summary>
    /// Allows management and editing of roles.
    /// </summary>
    ManageRoles = 1uL << 28,

    /// <summary>
    /// Allows management and editing of webhooks.
    /// </summary>
    ManageWebhooks = 1uL << 29,

    /// <summary>
    /// Allows for editing and deleting emojis, stickers, and soundboard sounds created by all users.
    /// </summary>
    ManageGuildExpressions = 1uL << 30,

    /// <summary>
    /// Allows users to use application commands, including slash commands and context menu commands.
    /// </summary>
    UseApplicationCommands = 1uL << 31,

    /// <summary>
    /// Allows for requesting to speak in stage channels.
    /// </summary>
    RequestToSpeak = 1uL << 32,

    /// <summary>
    /// Allows for creating, editing and deleting scheduled events created by all users.
    /// </summary>
    ManageEvents = 1uL << 33,

    /// <summary>
    /// Allows for deleting and archiving threads, and viewing all private threads.
    /// </summary>
    ManageThreads = 1uL << 34,

    /// <summary>
    /// Allows for creating public and announcement threads.
    /// </summary>
    CreatePublicThreads = 1uL << 35,

    /// <summary>
    /// Allows for creating private threads.
    /// </summary>
    CreatePrivateThreads = 1uL << 36,

    /// <summary>
    /// Allows the usage of custom stickers from other servers.
    /// </summary>
    UseExternalStickers = 1uL << 37,

    /// <summary>
    /// Allows for sending messages in threads.
    /// </summary>
    SendMessagesInThreads = 1uL << 38,

    /// <summary>
    /// Allows for using Activities (applications with the <see cref="ApplicationFlags.Embedded"/> flag) in a voice channel.
    /// </summary>
    StartEmbeddedActivities = 1uL << 39,

    /// <summary>
    /// Allows for timing out users to prevent them from sending or reacting to messages in chat and threads, and from speaking in voice and stage channels.
    /// </summary>
    ModerateUsers = 1uL << 40,

    /// <summary>
    /// Allows for viewing role subscription insights.
    /// </summary>
    ViewCreatorMonetizationAnalytics = 1uL << 41,

    /// <summary>
    /// Allows for using soundboard in a voice channel.
    /// </summary>
    UseSoundboard = 1uL << 42,

    /// <summary>
    /// Allows for creating emojis, stickers, and soundboard sounds, and editing and deleting those created by the current user.
    /// </summary>
    CreateGuildExpressions = 1uL << 43,

    /// <summary>
    /// Allows for creating scheduled events, and editing and deleting those created by the current user.
    /// </summary>
    CreateEvents = 1uL << 44,

    /// <summary>
    /// Allows the usage of custom soundboard sounds from other servers.
    /// </summary>
    UseExternalSounds = 1uL << 45,

    /// <summary>
    /// Allows sending voice messages.
    /// </summary>
    SendVoiceMessages = 1uL << 46,

    /// <summary>
    /// Allows sending polls.
    /// </summary>
    SendPolls = 1uL << 49,

    /// <summary>
    /// Allows user-installed apps to send public responses. When disabled, users will still be allowed to use their apps but the responses will be ephemeral. This only applies to apps not also installed to the server.
    /// </summary>
    UseExternalApplications = 1uL << 50,
}
