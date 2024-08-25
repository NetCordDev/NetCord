using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public partial class GuildEmoji(JsonEmoji jsonModel, ulong guildId, RestClient client) : CustomEmoji(jsonModel, client)
{
    public IReadOnlyList<ulong>? AllowedRoles => _jsonModel.AllowedRoles;

    public ulong GuildId { get; } = guildId;
}
