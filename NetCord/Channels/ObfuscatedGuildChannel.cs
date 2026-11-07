using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents an obfuscated channel within a guild. Channels are obfuscated when the bot does not have access to them. They are only received over the Gateway; the HTTP API omits inaccessible channels from responses.
/// </summary>
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
