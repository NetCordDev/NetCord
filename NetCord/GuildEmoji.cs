using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a custom guild emoji.
/// </summary>
public partial class GuildEmoji(JsonEmoji jsonModel, ulong guildId, RestClient client) : CustomEmoji(jsonModel, client)
{
    /// <summary>
    /// A list of roles allowed to use this emoji.
    /// </summary>
    public IReadOnlyList<ulong>? AllowedRoles => _jsonModel.AllowedRoles;

    /// <summary>
    /// The ID corresponding to the emoji's parent guild.
    /// </summary>
    public ulong GuildId { get; } = guildId;
}
