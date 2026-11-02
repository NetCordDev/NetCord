namespace NetCord.Rest;

public class FollowedChannel(JsonModels.JsonFollowedChannel jsonModel)
{
    public ulong ChannelId { get; } = jsonModel.ChannelId;

    public ulong WebhookId { get; } = jsonModel.WebhookId;
}
