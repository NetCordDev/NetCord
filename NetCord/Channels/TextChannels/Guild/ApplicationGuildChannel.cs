using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents an application channel.
/// </summary>
public partial class ApplicationGuildChannel(JsonModels.JsonChannel jsonModel, ulong guildId, RestClient client) : TextGuildChannel(jsonModel, guildId, client)
{
    /// <summary>
    /// The bound application's ID.
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> when the bound application has been removed from the guild.
    /// </remarks>
    public ulong? ApplicationId => _jsonModel.ApplicationId;
}
