using System.Runtime.CompilerServices;

using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a thread within a guild.
/// </summary>
public abstract partial class GuildThread(JsonModels.JsonChannel jsonModel, RestClient client) : TextGuildChannel(jsonModel, jsonModel.GuildId.GetValueOrDefault(), client), ICloneable
{
    /// <summary>
    /// The ID of the <see cref="TextGuildChannel"/> this thread was created in.
    /// </summary>
    public new ulong ParentId => base.ParentId.GetValueOrDefault();

    /// <summary>
    /// The ID of the thread's creator.
    /// </summary>
    public ulong OwnerId { get; } = jsonModel.OwnerId.GetValueOrDefault();

    /// <summary>
    /// The number of messages within the thread, excluding the initial and deleted messages.
    /// </summary>
    /// <remarks>
    /// For threads created before July 1, 2022, the message count is inaccurate when greater than 50.
    /// </remarks>
    public int MessageCount { get; } = jsonModel.MessageCount.GetValueOrDefault();

    /// <summary>
    /// An approximation of the number of users within the thread. Stops counting at 50 users.
    /// </summary>
    public int UserCount { get; } = jsonModel.UserCount.GetValueOrDefault();

    /// <summary>
    /// Additional metadata for the thread, unnecessary for standard channel operations.
    /// </summary>
    public GuildThreadMetadata Metadata { get; } = new(jsonModel.Metadata!);

    /// <summary>
    /// A minimal thread user object for the current user, if they have joined the thread.
    /// </summary>
    public ThreadCurrentUser? CurrentUser { get; set; } = jsonModel.CurrentUser is { } jsonCurrentUser ? new(jsonCurrentUser) : null;

    /// <summary>
    /// The total number of messages sent in the thread, including deletions.
    /// </summary>
    public int TotalMessageSent { get; } = jsonModel.TotalMessageSent.GetValueOrDefault();

    public static new GuildThread CreateFromJson(JsonModels.JsonChannel jsonChannel, RestClient client)
    {
        return jsonChannel.Type switch
        {
            ChannelType.AnnouncementGuildThread => new AnnouncementGuildThread(jsonChannel, client),
            ChannelType.PublicGuildThread => new PublicGuildThread(jsonChannel, client),
            ChannelType.PrivateGuildThread => new PrivateGuildThread(jsonChannel, client),
            _ => new UnknownGuildThread(jsonChannel, client),
        };
    }

    object ICloneable.Clone() => MemberwiseClone();

    internal GuildThread Clone() => Unsafe.As<GuildThread>(MemberwiseClone());
}
