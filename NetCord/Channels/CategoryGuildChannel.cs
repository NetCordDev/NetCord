using NetCord.Rest;

namespace NetCord;

public partial class CategoryGuildChannel(JsonModels.JsonChannel jsonModel, ulong guildId, RestClient client) : Channel(jsonModel, client), IGuildChannel
{
    public ulong GuildId { get; } = guildId;
    public int? Position => _jsonModel.Position;
    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; } = jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
    public string Name => _jsonModel.Name!;
}
