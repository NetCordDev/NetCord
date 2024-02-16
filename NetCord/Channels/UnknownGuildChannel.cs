using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

internal partial class UnknownGuildChannel(JsonChannel jsonModel, ulong guildId, RestClient client) : UnknownChannel(jsonModel, client), IUnknownGuildChannel
{
    public ulong GuildId { get; } = guildId;

    public int? Position => _jsonModel.Position;

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; } = jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));

    public string Name => _jsonModel.Name!;
}
