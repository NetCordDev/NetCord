using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public abstract class Channel : ClientEntity, IJsonModel<JsonChannel>, IInteractionChannel
{
    JsonChannel IJsonModel<JsonChannel>.JsonModel => _jsonModel;
    private protected JsonChannel _jsonModel;

    public override Snowflake Id => _jsonModel.Id;

    Permission IInteractionChannel.Permissions => _jsonModel.Permissions.GetValueOrDefault();

    private protected Channel(JsonChannel jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
    }

    public override string ToString() => $"<#{Id}>";

    public static Channel CreateFromJson(JsonChannel jsonChannel, RestClient client)
    {
        return jsonChannel.Type switch
        {
            ChannelType.TextGuildChannel => new TextGuildChannel(jsonChannel, client),
            ChannelType.DMChannel => new DMChannel(jsonChannel, client),
            ChannelType.VoiceGuildChannel => new VoiceGuildChannel(jsonChannel, client),
            ChannelType.GroupDMChannel => new GroupDMChannel(jsonChannel, client),
            ChannelType.CategoryChannel => new CategoryChannel(jsonChannel, client),
            ChannelType.NewsGuildChannel => new NewsGuildChannel(jsonChannel, client),
            ChannelType.NewsGuildThread => new NewsGuildThread(jsonChannel, client),
            ChannelType.PublicGuildThread => new PublicGuildThread(jsonChannel, client),
            ChannelType.PrivateGuildThread => new PrivateGuildThread(jsonChannel, client),
            ChannelType.StageGuildChannel => new StageGuildChannel(jsonChannel, client),
            ChannelType.DirectoryGuildChannel => new DirectoryGuildChannel(jsonChannel, client),
            ChannelType.ForumGuildChannel => new ForumGuildChannel(jsonChannel, client),
            _ => throw new ArgumentException($"Invalid '{nameof(jsonChannel.Type)}'."),
        };
    }

    #region Channel
    public async Task<Channel> DeleteAsync(RequestProperties? properties = null) => await _client.DeleteChannelAsync(Id, properties).ConfigureAwait(false);
    #endregion
}