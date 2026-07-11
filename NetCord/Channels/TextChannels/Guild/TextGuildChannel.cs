using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a text channel within a guild.
/// </summary>
public partial class TextGuildChannel(JsonModels.JsonChannel jsonModel, ulong guildId, RestClient client) : TextChannel(jsonModel, client), IGuildChannel
{
    public ulong GuildId { get; } = guildId;

    public int? Position => _jsonModel.Position;

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; } = jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));

    public string Name => _jsonModel.Name!;

    /// <summary>
    /// The channel topic, between 0 and 1024 characters. Can be <see langword="null"/>.
    /// </summary>
    public string? Topic => _jsonModel.Topic;

    /// <summary>
    /// Whether the channel is age-restricted.
    /// </summary>
    public bool Nsfw => _jsonModel.Nsfw.GetValueOrDefault();

    /// <summary>
    /// The number of seconds a user has to wait before sending another message, between 0 and 21600 (6 hours).
    /// </summary>
    /// <remarks>
    /// Bots, and users with the <see cref="Permissions.BypassSlowmode"/> permission are unaffected.
    /// </remarks>
    public int Slowmode => _jsonModel.Slowmode.GetValueOrDefault();

    /// <summary>
    /// The ID of the channel's parent category.
    /// </summary>
    public ulong? ParentId => _jsonModel.ParentId;

    /// <summary>
    /// How long threads within the channel must be inactive, before auto-archiving occurs.
    /// </summary>
    public ThreadArchiveDuration? DefaultAutoArchiveDuration => _jsonModel.DefaultAutoArchiveDuration;

    /// <summary>
    /// The default slowmode duration for threads within the channel.
    /// </summary>
    public int DefaultThreadSlowmode => _jsonModel.DefaultThreadSlowmode.GetValueOrDefault();
}
