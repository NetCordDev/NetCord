using NetCord.JsonModels;

namespace NetCord;

public abstract class Channel : ClientEntity
{
    private protected JsonChannel _jsonEntity;

    public override DiscordId Id => _jsonEntity.Id;

    private protected Channel(JsonChannel jsonEntity, RestClient client) : base(client)
    {
        _jsonEntity = jsonEntity;
    }

    public override string ToString() => $"<#{Id}>";

    internal static Channel CreateFromJson(JsonChannel jsonChannel, RestClient client)
    {
        return jsonChannel.Type switch
        {
            ChannelType.TextGuildChannel => new TextGuildChannel(jsonChannel, client),
            ChannelType.DMChannel => new DMChannel(jsonChannel, client),
            ChannelType.GuildVoiceChannel => new VoiceGuildChannel(jsonChannel, client),
            ChannelType.GroupDMChannel => new GroupDMChannel(jsonChannel, client),
            ChannelType.CategoryChannel => new CategoryChannel(jsonChannel, client),
            ChannelType.NewsGuildChannel => new NewsGuildChannel(jsonChannel, client),
            ChannelType.StoreGuildChannel => new StoreGuildChannel(jsonChannel, client),
            ChannelType.NewsGuildThread => new NewsThread(jsonChannel, client),
            ChannelType.PublicGuildThread => new PublicThread(jsonChannel, client),
            ChannelType.PrivateGuildThread => new PrivateThread(jsonChannel, client),
            ChannelType.StageGuildChannel => new StageGuildChannel(jsonChannel, client),
            _ => throw new ArgumentException($"Invalid {nameof(jsonChannel.Type)}"),
        };
    }
}