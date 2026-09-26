using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a media channel, which is a specialized <see cref="ForumGuildChannel"/>.
/// </summary>
public partial class MediaForumGuildChannel(JsonChannel jsonModel, ulong guildId, RestClient client) : ForumGuildChannel(jsonModel, guildId, client)
{
}
