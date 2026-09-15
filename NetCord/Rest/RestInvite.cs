using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

/// <summary>
/// Represents a REST invite.
/// </summary>
public partial class RestInvite(JsonRestInvite jsonModel, RestClient client) : IInvite, IJsonModel<JsonRestInvite>
{
    JsonRestInvite IJsonModel<JsonRestInvite>.JsonModel => jsonModel;

    /// <summary>
    /// The type of the invite.
    /// </summary>
    public InviteType Type => jsonModel.Type;

    /// <summary>
    /// The invite code.
    /// </summary>
    public string Code => jsonModel.Code;

    /// <summary>
    /// The guild this invite is for.
    /// </summary>
    public RestGuild? Guild { get; } = jsonModel.Guild is { } guild ? new(guild, client) : null;

    /// <summary>
    /// The channel this invite is for.
    /// </summary>
    public Channel? Channel { get; } = jsonModel.Channel is { } channel ? Channel.CreateFromJson(channel, client) : null;

    /// <summary>
    /// The user who created the invite.
    /// </summary>
    public User? Inviter { get; } = jsonModel.Inviter is { } inviter ? new(inviter, client) : null;

    /// <summary>
    /// The type of target for this voice channel invite.
    /// </summary>
    public InviteTargetType? TargetType => jsonModel.TargetType;

    /// <summary>
    /// The user target for this invite.
    /// </summary>
    public User? TargetUser { get; } = jsonModel.TargetUser is { } targetUser ? new(targetUser, client) : null;

    /// <summary>
    /// The application target for this invite.
    /// </summary>
    public Application? TargetApplication { get; } = jsonModel.TargetApplication is { } targetApplication ? new(targetApplication, client) : null;

    /// <summary>
    /// Approximate count of online members.
    /// </summary>
    public int? ApproximatePresenceCount => jsonModel.ApproximatePresenceCount;

    /// <summary>
    /// Approximate count of total members.
    /// </summary>
    public int? ApproximateUserCount => jsonModel.ApproximateUserCount;

    /// <summary>
    /// The expiration date of this invite.
    /// </summary>
    public DateTimeOffset? ExpiresAt => jsonModel.ExpiresAt;

    /// <summary>
    /// Guild scheduled event data.
    /// </summary>
    public GuildScheduledEvent? GuildScheduledEvent { get; } = jsonModel.GuildScheduledEvent is { } guildScheduledEvent ? new(guildScheduledEvent, client) : null;

    /// <summary>
    /// Flags for the invite.
    /// </summary>
    public InviteFlags? Flags => jsonModel.Flags;

    /// <summary>
    /// Roles of the partial guild for this invite.
    /// </summary>
    public IReadOnlyList<Role>? Roles { get; } = jsonModel.Guild is { } guildJson && jsonModel.Roles is { } roles ? roles.Select(role => new Role(role, guildJson.Id, client)).ToArray() : null;

    // Metadata

    /// <summary>
    /// Number of times this invite has been used.
    /// </summary>
    public int? Uses => jsonModel.Uses;

    /// <summary>
    /// Max number of times this invite can be used.
    /// </summary>
    public int? MaxUses => jsonModel.MaxUses;

    /// <summary>
    /// Duration (in seconds) after which the invite expires.
    /// </summary>
    public int? MaxAge => jsonModel.MaxAge;

    /// <summary>
    /// Whether this invite only grants temporary membership.
    /// </summary>
    public bool? Temporary => jsonModel.Temporary;

    /// <summary>
    /// When this invite was created.
    /// </summary>
    public DateTimeOffset? CreatedAt => jsonModel.CreatedAt;

    ulong? IInvite.GuildId => Guild?.Id;

    ulong? IInvite.ChannelId => Channel?.Id;
}
