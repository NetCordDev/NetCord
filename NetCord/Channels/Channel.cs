using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class Channel : ClientEntity, IJsonModel<JsonChannel>, IInteractionChannel
{
    JsonChannel IJsonModel<JsonChannel>.JsonModel => _jsonModel;
    private protected JsonChannel _jsonModel;

    public override ulong Id => _jsonModel.Id;
    public ChannelFlags Flags => _jsonModel.Flags.GetValueOrDefault();

    Permissions IInteractionChannel.Permissions => _jsonModel.Permissions.GetValueOrDefault();

    public Channel(JsonChannel jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
    }

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => Mention.TryFormatChannel(destination, out charsWritten, Id);

    public static Channel CreateFromJson(JsonChannel jsonChannel, RestClient client)
    {
        return jsonChannel.Type switch
        {
            ChannelType.TextGuildChannel => new TextGuildChannel(jsonChannel, client),
            ChannelType.DMChannel => new DMChannel(jsonChannel, client),
            ChannelType.VoiceGuildChannel => new VoiceGuildChannel(jsonChannel, client),
            ChannelType.GroupDMChannel => new GroupDMChannel(jsonChannel, client),
            ChannelType.CategoryChannel => new CategoryGuildChannel(jsonChannel, client),
            ChannelType.AnnouncementGuildChannel => new AnnouncementGuildChannel(jsonChannel, client),
            ChannelType.AnnouncementGuildThread => new AnnouncementGuildThread(jsonChannel, client),
            ChannelType.PublicGuildThread => new PublicGuildThread(jsonChannel, client),
            ChannelType.PrivateGuildThread => new PrivateGuildThread(jsonChannel, client),
            ChannelType.StageGuildChannel => new StageGuildChannel(jsonChannel, client),
            ChannelType.DirectoryGuildChannel => new DirectoryGuildChannel(jsonChannel, client),
            ChannelType.ForumGuildChannel => new ForumGuildChannel(jsonChannel, client),
            ChannelType.MediaForumGuildChannel => new MediaForumGuildChannel(jsonChannel, client),
            _ => new Channel(jsonChannel, client),
        };
    }

    #region Channel
    public async Task<Channel> DeleteAsync(RequestProperties? properties = null) => await _client.DeleteChannelAsync(Id, properties).ConfigureAwait(false);
    #endregion
}
