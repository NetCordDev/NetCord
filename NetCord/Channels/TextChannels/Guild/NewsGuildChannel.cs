using NetCord.Rest;

namespace NetCord;

public class NewsGuildChannel : TextGuildChannel
{
    public NewsGuildChannel(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
    }

    #region Channel
    public Task<RestMessage> CrosspostMessageAsync(Snowflake messageId, RequestProperties? properties = null) => _client.CrosspostMessageAsync(Id, messageId, properties);
    public Task<FollowedChannel> FollowAsync(Snowflake targetChannelId, RequestProperties? properties = null) => _client.FollowNewsGuildChannelAsync(Id, targetChannelId, properties);
    #endregion
}
