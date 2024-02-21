using System.Globalization;

using NetCord.JsonModels;

namespace NetCord.Rest;

public class GuildTemplatePreview(JsonGuild jsonModel, RestClient client) : IJsonModel<JsonGuild>
{
    JsonGuild IJsonModel<JsonGuild>.JsonModel => jsonModel;

    public string Name => jsonModel.Name;
    public string? IconHash => jsonModel.IconHashTemplate;
    public string? Description => jsonModel.Description;
    public VerificationLevel VerificationLevel => jsonModel.VerificationLevel;
    public DefaultMessageNotificationLevel DefaultMessageNotificationLevel => jsonModel.DefaultMessageNotificationLevel;
    public ContentFilter ContentFilter => jsonModel.ContentFilter;
    public CultureInfo PreferredLocale => jsonModel.PreferredLocale;
    public ulong? AfkChannelId => jsonModel.AfkChannelId;
    public int AfkTimeout => jsonModel.AfkTimeout;
    public ulong? SystemChannelId => jsonModel.SystemChannelId;
    public SystemChannelFlags SystemChannelFlags => jsonModel.SystemChannelFlags;
    public IReadOnlyDictionary<ulong, Role> Roles { get; } = jsonModel.Roles.ToDictionaryOrEmpty(r => new Role(r, 0, client));
    public IReadOnlyDictionary<ulong, IGuildChannel> Channels { get; } = jsonModel.Channels.ToDictionaryOrEmpty(c => IGuildChannel.CreateFromJson(c, 0, client));
}
