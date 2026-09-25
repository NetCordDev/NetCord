using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a forum channel within a guild.
/// </summary>
public partial class ForumGuildChannel : Channel, IGuildChannel
{
    public ForumGuildChannel(JsonChannel jsonModel, ulong guildId, RestClient client) : base(jsonModel, client)
    {
        GuildId = guildId;
        PermissionOverwrites = jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
        AvailableTags = jsonModel.AvailableTags.SelectOrEmpty(t => new ForumTag(t)).ToArray();

        var defaultReactionEmoji = jsonModel.DefaultReactionEmoji;
        if (defaultReactionEmoji is not null)
            DefaultReactionEmoji = new(defaultReactionEmoji);
    }

    public ulong GuildId { get; }
    public int? Position => _jsonModel.Position;
    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; }
    public string Name => _jsonModel.Name!;

    /// <summary>
    /// The channel topic, between 0 and 4096 characters. Can be <see langword="null"/>.
    /// </summary>
    public string? Topic => _jsonModel.Topic;

    /// <inheritdoc cref="TextGuildChannel.Nsfw"/>
    public bool Nsfw => _jsonModel.Nsfw.GetValueOrDefault();

    /// <summary>
    /// The ID of the last thread created in this channel. Can be <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// May not point to an existing or valid thread.
    /// </remarks>
    public ulong? LastThreadId => _jsonModel.LastMessageId;

    /// <summary>
    /// The number of seconds a user has to wait before creating another thread, between 0 and 21600 (6 hours).
    /// </summary>
    /// <remarks>
    /// Bots, and users with the <see cref="Permissions.BypassSlowmode"/> permission are unaffected.
    /// </remarks>
    public int Slowmode => _jsonModel.Slowmode.GetValueOrDefault();

    /// <inheritdoc cref="TextGuildChannel.ParentId"/>
    public ulong? ParentId => _jsonModel.ParentId;

    /// <summary>
    /// When the last pinned message was pinned. Can be <see langword="null"/>.
    /// </summary>
    public DateTimeOffset? LastPin => _jsonModel.LastPin;

    /// <summary>
    /// The set of tags available for use in the channel.
    /// </summary>
    public IReadOnlyList<ForumTag> AvailableTags { get; }

    /// <summary>
    /// The emoji to display by default as the add reaction button.
    /// </summary>
    public ForumGuildChannelDefaultReaction? DefaultReactionEmoji { get; }

    /// <inheritdoc cref="TextGuildChannel.DefaultThreadSlowmode"/>.
    public int DefaultThreadSlowmode => _jsonModel.DefaultThreadSlowmode.GetValueOrDefault();

    /// <summary>
    /// The sort order to use for threads within the channel.
    /// </summary>
    /// <remarks>
    /// If set to <see langword="null"/>, indicates no preferred sort order has been set.
    /// </remarks>
    public SortOrderType? DefaultSortOrder => _jsonModel.DefaultSortOrder;

    /// <summary>
    /// The default layout to use for threads within the channel.
    /// </summary>
    public ForumLayoutType DefaultForumLayout => _jsonModel.DefaultForumLayout.GetValueOrDefault();
}
