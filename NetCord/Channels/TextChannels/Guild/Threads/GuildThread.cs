using NetCord.Rest;

namespace NetCord;

public abstract partial class GuildThread : TextGuildChannel
{
    public ulong OwnerId => _jsonModel.OwnerId.GetValueOrDefault();
    public int MessageCount => _jsonModel.MessageCount.GetValueOrDefault();
    public int UserCount => _jsonModel.UserCount.GetValueOrDefault();
    public GuildThreadMetadata Metadata { get; }
    public ThreadCurrentUser? CurrentUser { get; }
    public int TotalMessageSent => _jsonModel.TotalMessageSent.GetValueOrDefault();

    protected GuildThread(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, jsonModel.GuildId.GetValueOrDefault(), client)
    {
        Metadata = new(jsonModel.Metadata!);

        var jsonCurrentUser = jsonModel.CurrentUser;
        if (jsonCurrentUser is not null)
            CurrentUser = new(jsonCurrentUser);
    }

    public static new GuildThread CreateFromJson(JsonModels.JsonChannel jsonChannel, RestClient client)
    {
        return jsonChannel.Type switch
        {
            ChannelType.AnnouncementGuildThread => new AnnouncementGuildThread(jsonChannel, client),
            ChannelType.PublicGuildThread => new PublicGuildThread(jsonChannel, client),
            ChannelType.PrivateGuildThread => new PrivateGuildThread(jsonChannel, client),
            _ => new UnknownGuildThread(jsonChannel, client),
        };
    }
}
