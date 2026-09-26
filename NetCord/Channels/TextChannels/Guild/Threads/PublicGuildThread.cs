using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a <see cref="GuildThread"/> accessible to all users.
/// </summary>
public partial class PublicGuildThread(JsonModels.JsonChannel jsonModel, RestClient client) : GuildThread(jsonModel, client)
{
    /// <summary>
    /// The set of tags applied to the thread.
    /// </summary>
    /// <remarks>
    /// Only available if the thread is within a <see cref="ForumGuildChannel"/> or <see cref="MediaForumGuildChannel"/>.
    /// </remarks>
    public IReadOnlyList<ulong>? AppliedTags => _jsonModel.AppliedTags;
}
