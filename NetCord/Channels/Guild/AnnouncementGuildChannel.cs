using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a channel that users can follow and crosspost from into their own servers. Formerly known as news channels.
/// </summary>
internal partial class AnnouncementGuildChannel(JsonModels.JsonChannel jsonModel, ulong guildId, RestClient client) : GuildMessageChannelBase(jsonModel, guildId, client), IAnnouncementGuildChannel
{
    public string? Topic => _jsonModel.Topic;

    public bool? Nsfw => _jsonModel.Nsfw;

    public int? Slowmode => _jsonModel.Slowmode;

    public ThreadArchiveDuration? DefaultAutoArchiveDuration => _jsonModel.DefaultAutoArchiveDuration;

    public string Name => _jsonModel.Name!;

    public int Position => _jsonModel.Position.GetValueOrDefault();

    public ulong? ParentId => _jsonModel.ParentId;
    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites => _jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
    public DateTimeOffset? LastPin => _jsonModel.LastPin;
}
