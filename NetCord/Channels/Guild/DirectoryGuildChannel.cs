using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a hub channel, with listed guilds.
/// </summary>
internal partial class DirectoryGuildChannel(JsonChannel jsonModel, ulong guildId, RestClient client) : TextChannelBase(jsonModel, client), IGuildChannel
{
    public ulong GuildId { get; } = guildId;
    public int? Position => _jsonModel.Position;
    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites => _jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
    public string Name => _jsonModel.Name!;
}
