using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class GuildTemplateSerializerSourceGuild(JsonGuildTemplateSerializedSourceGuild jsonModel, RestClient client)
{
    public string Name { get; } = jsonModel.Name;

    public string? Description { get; } = jsonModel.Description;

    public VerificationLevel? VerificationLevel { get; } = jsonModel.VerificationLevel;

    public DefaultMessageNotificationLevel? DefaultMessageNotificationLevel { get; } = jsonModel.DefaultMessageNotificationLevel;

    public ContentFilter? ExplicitContentFilterLevel { get; } = jsonModel.ExplicitContentFilterLevel;

    public string PreferredLocale { get; } = jsonModel.PreferredLocale;

    public int? AfkTimeout { get; } = jsonModel.AfkTimeout;

    public IReadOnlyDictionary<ulong, Role> Roles { get; } = jsonModel.Roles.ToDictionary(r => r.Id, r => new Role(r, 0, client));

    public IReadOnlyDictionary<ulong, IGuildChannel> Channels { get; } = jsonModel.Channels.ToDictionary(c => c.Id, c => IGuildChannel.CreateFromJson(c, 0, client));

    public ulong? AfkChannelId { get; } = jsonModel.AfkChannelId;

    public ulong? SystemChannelId { get; } = jsonModel.SystemChannelId;

    public SystemChannelFlags? SystemChannelFlags { get; } = jsonModel.SystemChannelFlags;

    public string? IconHash { get; } = jsonModel.IconHash;
}
