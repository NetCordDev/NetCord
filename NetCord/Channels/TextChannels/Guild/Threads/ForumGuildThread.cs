using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a <see cref="GuildThread"/> within a <see cref="ForumGuildChannel"/>.
/// </summary>
public partial class ForumGuildThread(JsonChannel jsonModel, RestClient client) : PublicGuildThread(jsonModel, client)
{
    /// <summary>
    /// The message embedded as the thread's starting point.
    /// </summary>
    public RestMessage Message { get; } = new(jsonModel.Message!, client);
}
