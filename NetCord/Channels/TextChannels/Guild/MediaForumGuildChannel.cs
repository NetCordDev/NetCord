using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class MediaForumGuildChannel : ForumGuildChannel
{
    public MediaForumGuildChannel(JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
    }
}
