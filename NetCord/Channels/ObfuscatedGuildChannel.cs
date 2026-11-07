using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public partial class ObfuscatedGuildChannel(JsonChannel jsonModel, ulong guildId, RestClient client) : Channel(jsonModel, client), IGuildChannel
{
    public ulong GuildId => guildId;

    public int? Position => _jsonModel.Position;

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; } = jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(o => o.Id, o => new PermissionOverwrite(o));

    public string Name => _jsonModel.Name!;

    /// <summary>
    /// The ID of the channel's parent channel.
    /// </summary>
    public ulong? ParentId => _jsonModel.ParentId;
}
