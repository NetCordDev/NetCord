using NetCord.Gateway.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class Invite(JsonInvite jsonModel, RestClient client) : IInvite
{
    public InviteType Type { get; } = jsonModel.Type;

    public ulong ChannelId { get; } = jsonModel.ChannelId;

    public string Code { get; } = jsonModel.Code;

    public DateTimeOffset CreatedAt { get; } = jsonModel.CreatedAt;

    public ulong? GuildId { get; } = jsonModel.GuildId;

    public User? Inviter { get; } = jsonModel.Inviter is { } inviter ? new(inviter, client) : null;

    public int MaxAge { get; } = jsonModel.MaxAge;

    public int MaxUses { get; } = jsonModel.MaxUses;

    public InviteTargetType? TargetType { get; } = jsonModel.TargetType;

    public User? TargetUser { get; } = jsonModel.TargetUser is { } targetUser ? new(targetUser, client) : null;

    public PartialApplication? TargetApplication { get; } = jsonModel.TargetApplication is { } targetApplication ? new(targetApplication, client) : null;

    public bool Temporary { get; } = jsonModel.Temporary;

    public int Uses { get; } = jsonModel.Uses;

    public DateTimeOffset? ExpiresAt { get; } = jsonModel.ExpiresAt;

    public IReadOnlyList<ulong>? RoleIds { get; } = jsonModel.RoleIds;

    ulong? IInvite.ChannelId => ChannelId;

    int? IInvite.MaxAge => MaxAge;

    int? IInvite.MaxUses => MaxUses;

    bool? IInvite.Temporary => Temporary;

    int? IInvite.Uses => Uses;

    DateTimeOffset? IInvite.CreatedAt => CreatedAt;
}
