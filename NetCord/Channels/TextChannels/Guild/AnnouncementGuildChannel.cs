using NetCord.Rest;

namespace NetCord;

public class AnnouncementGuildChannel : TextGuildChannel
{
    public AnnouncementGuildChannel(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
    }

    #region Channel
    public Task<RestMessage> CrosspostMessageAsync(Snowflake messageId, RequestProperties? properties = null) => _client.CrosspostMessageAsync(Id, messageId, properties);
    public Task<FollowedChannel> FollowAsync(Snowflake targetChannelId, RequestProperties? properties = null) => _client.FollowAnnouncementGuildChannelAsync(Id, targetChannelId, properties);
    #endregion
}
