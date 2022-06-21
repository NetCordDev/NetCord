using System.Collections.Immutable;

using NetCord.JsonModels;

namespace NetCord.Rest;

public class GuildTemplateSourceGuild : IJsonModel<JsonGuild>
{
    JsonGuild IJsonModel<JsonGuild>.JsonModel => _jsonModel;
    private readonly JsonGuild _jsonModel;

    public GuildTemplateSourceGuild(JsonGuild jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        Roles = jsonModel.Roles.ToImmutableDictionaryOrEmpty(r => r.Id, r => new GuildRole(r, client));
        Channels = _jsonModel.Channels.ToImmutableDictionary(c => c.Id, c => (IGuildChannel)Channel.CreateFromJson(c, client));
    }

    public string Name => _jsonModel.Name;

    public Snowflake? AfkChannelId => _jsonModel.AfkChannelId;

    public int AfkTimeout => _jsonModel.AfkTimeout;

    public VerificationLevel VerificationLevel => _jsonModel.VerificationLevel;

    public DefaultMessageNotificationLevel DefaultMessageNotificationLevel => _jsonModel.DefaultMessageNotificationLevel;

    public ContentFilter ContentFilter => _jsonModel.ContentFilter;

    public ImmutableDictionary<Snowflake, GuildRole> Roles { get; internal set; }

    public Snowflake? SystemChannelId => _jsonModel.SystemChannelId;

    public SystemChannelFlags SystemChannelFlags => _jsonModel.SystemChannelFlags;

    public ImmutableDictionary<Snowflake, IGuildChannel> Channels { get; internal set; }

    public string? Description => _jsonModel.Description;

    public System.Globalization.CultureInfo PreferredLocale => _jsonModel.PreferredLocale;
}