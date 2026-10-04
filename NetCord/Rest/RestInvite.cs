using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class RestInviteChannel(JsonRestInviteChannel jsonModel) : Entity(jsonModel)
{
    public string Name { get; } = jsonModel.Name;

    public ChannelType Type { get; } = jsonModel.Type;
}

public partial class RestInvite(JsonRestInvite jsonModel, RestClient client) : IInvite
{
    public InviteType Type { get; } = jsonModel.Type;

    public string Code { get; } = jsonModel.Code;

    public PartialGuild? Guild { get; } = jsonModel.Guild is { } guild ? new(guild, client) : null;

    public RestInviteChannel? Channel { get; } = jsonModel.Channel is { } channel ? new(channel) : null;

    public User? Inviter { get; } = jsonModel.Inviter is { } inviter ? new(inviter, client) : null;

    public InviteTargetType? TargetType { get; } = jsonModel.TargetType;

    public User? TargetUser { get; } = jsonModel.TargetUser is { } targetUser ? new(targetUser, client) : null;

    public PartialApplication? TargetApplication { get; } = jsonModel.TargetApplication is { } targetApplication ? new(targetApplication, client) : null;

    public int? ApproximatePresenceCount { get; } = jsonModel.ApproximatePresenceCount;

    public int? ApproximateUserCount { get; } = jsonModel.ApproximateUserCount;

    public DateTimeOffset? ExpiresAt { get; } = jsonModel.ExpiresAt;

    public GuildScheduledEvent? GuildScheduledEvent { get; } = jsonModel.GuildScheduledEvent is { } guildScheduledEvent ? new(guildScheduledEvent, client) : null;

    public InviteFlags? Flags { get; } = jsonModel.Flags;

    public IReadOnlyList<Role>? Roles { get; } = jsonModel.Roles is { } roles && jsonModel.Guild is { } guild ? roles.Select(role => new Role(role, guild.Id, client)).ToArray() : null;

    // Metadata
    public int? Uses { get; } = jsonModel.Uses;

    public int? MaxUses { get; } = jsonModel.MaxUses;

    public int? MaxAge { get; } = jsonModel.MaxAge;

    public bool? Temporary { get; } = jsonModel.Temporary;

    public DateTimeOffset? CreatedAt { get; } = jsonModel.CreatedAt;

    ulong? IInvite.GuildId => Guild?.Id;

    ulong? IInvite.ChannelId => Channel?.Id;
}
