using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a <see cref="GuildThread"/> within a <see cref="ForumGuildChannel"/>, acquired via <see cref="RestClient.CreateForumGuildThreadAsync(ulong, ForumGuildThreadProperties, RestRequestProperties?, CancellationToken)"/>.
/// </summary>
/// <remarks>
/// Threads within a forum are typically acquired as <see cref="PublicGuildThread"/> objects.
/// </remarks>
public partial class ForumGuildThread(JsonChannel jsonModel, RestClient client) : PublicGuildThread(jsonModel, client)
{
    /// <summary>
    /// The message embedded as the thread's starting point.
    /// </summary>
    public RestMessage Message { get; } = new(jsonModel.Message!, client);
}
