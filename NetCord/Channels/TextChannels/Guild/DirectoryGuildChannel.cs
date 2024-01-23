using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public partial class DirectoryGuildChannel : TextChannel, IGuildChannel
{
    public DirectoryGuildChannel(JsonChannel jsonModel, ulong guildId, RestClient client) : base(jsonModel, client)
    {
        GuildId = guildId;
        PermissionOverwrites = jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
    }

    public ulong GuildId { get; }
    public int? Position => _jsonModel.Position;
    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; }
    public string Name => _jsonModel.Name!;
}
