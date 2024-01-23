using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public partial class MediaForumGuildChannel : ForumGuildChannel
{
    public MediaForumGuildChannel(JsonChannel jsonModel, ulong guildId, RestClient client) : base(jsonModel, guildId, client)
    {
    }
}
