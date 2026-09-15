using NetCord.Rest;

namespace NetCord.Gateway;

/// <summary>
/// Represents a Gateway invite object (such as from invite events).
/// </summary>
public class Invite(JsonModels.JsonInvite jsonModel, RestClient client) : IInvite, IJsonModel<JsonModels.JsonInvite>
{
    JsonModels.JsonInvite IJsonModel<JsonModels.JsonInvite>.JsonModel => jsonModel;

    /// <summary>
    /// The type of the invite.
    /// </summary>
    public InviteType Type => jsonModel.Type;

    /// <summary>
    /// The channel that the invite points to.
    /// </summary>
    public ulong ChannelId => jsonModel.ChannelId;

    /// <summary>
    /// The invite code (unique ID).
    /// </summary>
    public string Code => jsonModel.Code;

    /// <summary>
    /// The time at which the invite was created.
    /// </summary>
    public DateTimeOffset CreatedAt => jsonModel.CreatedAt;

    /// <summary>
    /// The guild that the invite points to.
    /// </summary>
    public ulong? GuildId => jsonModel.GuildId;

    /// <summary>
    /// The user who created the invite.
    /// </summary>
    public User? Inviter { get; } = jsonModel.Inviter is { } inviter ? new(inviter, client) : null;

    /// <summary>
    /// How long the invite is valid for (in seconds).
    /// </summary>
    public int MaxAge => jsonModel.MaxAge;

    /// <summary>
    /// The maximum number of times the invite can be used.
    /// </summary>
    public int MaxUses => jsonModel.MaxUses;

    /// <summary>
    /// The type of target for this voice channel invite.
    /// </summary>
    public InviteTargetType? TargetType => jsonModel.TargetType;

    /// <summary>
    /// The user whose stream to display for this voice channel stream invite.
    /// </summary>
    public User? TargetUser { get; } = jsonModel.TargetUser is { } targetUser ? new(targetUser, client) : null;

    /// <summary>
    /// The embedded application to open for this voice channel embedded application invite.
    /// </summary>
    public Application? TargetApplication { get; }
        = jsonModel.TargetApplication is { } targetApplication ? new(targetApplication, client) : null;

    /// <summary>
    /// Whether the invite only grants temporary membership.
    /// </summary>
    public bool Temporary => jsonModel.Temporary;

    /// <summary>
    /// How many times the invite has been used.
    /// </summary>
    public int Uses => jsonModel.Uses;

    /// <summary>
    /// The expiration date of this invite, taking into account <see cref="MaxAge"/>.
    /// </summary>
    public DateTimeOffset? ExpiresAt => jsonModel.ExpiresAt;

    /// <summary>
    /// The IDs of roles that will be assigned to the user when joining the guild via this invite.
    /// </summary>
    public IReadOnlyList<ulong>? RoleIds => jsonModel.RoleIds;

    ulong? IInvite.ChannelId => ChannelId;

    int? IInvite.MaxAge => MaxAge;

    int? IInvite.MaxUses => MaxUses;

    bool? IInvite.Temporary => Temporary;

    int? IInvite.Uses => Uses;

    DateTimeOffset? IInvite.CreatedAt => CreatedAt;
}
