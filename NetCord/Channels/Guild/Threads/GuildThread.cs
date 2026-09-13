using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a thread within a guild.
/// </summary>
internal abstract partial class GuildThread : GuildMessageChannelBase, IGuildThread
{
    /// <summary>
    /// The ID of the parent channel of the thread.
    /// </summary>
    public ulong ParentId => _jsonModel.ParentId.GetValueOrDefault();

    /// <summary>
    /// The number of messages within the thread, excluding the initial and deleted messages.
    /// </summary>
    /// <remarks>
    /// For threads created before July 1, 2022, the message count is inaccurate when greater than 50.
    /// </remarks>
    public int MessageCount => _jsonModel.MessageCount.GetValueOrDefault();

    /// <summary>
    /// An approximation of the number of users within the thread. Stops counting at 50 users.
    /// </summary>
    public int UserCount => _jsonModel.UserCount.GetValueOrDefault();

    /// <summary>
    /// Additional metadata for the thread, unnecessary for standard channel operations.
    /// </summary>
    public GuildThreadMetadata Metadata { get; }

    /// <summary>
    /// A minimal thread user object for the current user, if they have joined the thread.
    /// </summary>
    public ThreadCurrentUser? CurrentUser { get; }

    /// <summary>
    /// The total number of messages sent in the thread, including deletions.
    /// </summary>
    public int TotalMessageSent => _jsonModel.TotalMessageSent.GetValueOrDefault();

    /// <summary>
    /// The name of the thread.
    /// </summary>
    public string Name => _jsonModel.Name!;

    /// <summary>
    /// The timestamp of the last pinned message in the thread.
    /// </summary>
    public DateTimeOffset? LastPin => _jsonModel.LastPin;

    /// <summary>
    /// The ID of the user who created the thread.
    /// </summary>
    public ulong OwnerId => _jsonModel.OwnerId.GetValueOrDefault();

    protected GuildThread(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, jsonModel.GuildId.GetValueOrDefault(), client)
    {
        Metadata = new(jsonModel.Metadata!);

        var jsonCurrentUser = jsonModel.CurrentUser;
        if (jsonCurrentUser is not null)
            CurrentUser = new(jsonCurrentUser);
    }
}
