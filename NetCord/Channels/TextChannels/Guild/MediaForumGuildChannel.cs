using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public partial class MediaForumGuildChannel(JsonChannel jsonModel, ulong guildId, RestClient client) : ForumGuildChannel(jsonModel, guildId, client)
{
}
