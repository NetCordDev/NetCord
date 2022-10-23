using System.Collections.Immutable;

using NetCord.JsonModels;

namespace NetCord.Rest;

public class GuildTemplateSourceGuild : Entity, IJsonModel<JsonGuild>
{
    JsonGuild IJsonModel<JsonGuild>.JsonModel => _jsonModel;
    private readonly JsonGuild _jsonModel;

    public GuildTemplateSourceGuild(JsonGuild jsonModel, ulong id, RestClient client)
    {
        _jsonModel = jsonModel;
        Roles = jsonModel.Roles.ToImmutableDictionaryOrEmpty(r => new GuildRole(r, id, client));
        Channels = _jsonModel.Channels.ToImmutableDictionary(c => (IGuildChannel)Channel.CreateFromJson(c, client));
        Id = id;
    }

    public override ulong Id { get; }

    public string Name => _jsonModel.Name;

    public ulong? AfkChannelId => _jsonModel.AfkChannelId;

    public int AfkTimeout => _jsonModel.AfkTimeout;

    public VerificationLevel VerificationLevel => _jsonModel.VerificationLevel;

    public DefaultMessageNotificationLevel DefaultMessageNotificationLevel => _jsonModel.DefaultMessageNotificationLevel;

    public ContentFilter ContentFilter => _jsonModel.ContentFilter;

    public ImmutableDictionary<ulong, GuildRole> Roles { get; internal set; }

    public ulong? SystemChannelId => _jsonModel.SystemChannelId;

    public SystemChannelFlags SystemChannelFlags => _jsonModel.SystemChannelFlags;

    public ImmutableDictionary<ulong, IGuildChannel> Channels { get; internal set; }

    public string? Description => _jsonModel.Description;

    public System.Globalization.CultureInfo PreferredLocale => _jsonModel.PreferredLocale;
}
