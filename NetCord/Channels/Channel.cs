using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public abstract partial class Channel(JsonChannel jsonModel, RestClient client) : ClientEntity(client), IJsonModel<JsonChannel>, IInteractionChannel
{
    JsonChannel IJsonModel<JsonChannel>.JsonModel => _jsonModel;
    private protected JsonChannel _jsonModel = jsonModel;

    public override ulong Id => _jsonModel.Id;
    public ChannelFlags Flags => _jsonModel.Flags.GetValueOrDefault();

    Permissions IInteractionChannel.Permissions => _jsonModel.Permissions.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => Mention.TryFormatChannel(destination, out charsWritten, Id);

    public static Channel CreateFromJson(JsonChannel jsonChannel, RestClient client)
    {
        return jsonChannel.Type switch
        {
            ChannelType.TextGuildChannel => new TextGuildChannel(jsonChannel, jsonChannel.GuildId.GetValueOrDefault(), client),
            ChannelType.DMChannel => new DMChannel(jsonChannel, client),
            ChannelType.VoiceGuildChannel => new VoiceGuildChannel(jsonChannel, jsonChannel.GuildId.GetValueOrDefault(), client),
            ChannelType.GroupDMChannel => new GroupDMChannel(jsonChannel, client),
            ChannelType.CategoryChannel => new CategoryGuildChannel(jsonChannel, jsonChannel.GuildId.GetValueOrDefault(), client),
            ChannelType.AnnouncementGuildChannel => new AnnouncementGuildChannel(jsonChannel, jsonChannel.GuildId.GetValueOrDefault(), client),
            ChannelType.AnnouncementGuildThread => new AnnouncementGuildThread(jsonChannel, client),
            ChannelType.PublicGuildThread => new PublicGuildThread(jsonChannel, client),
            ChannelType.PrivateGuildThread => new PrivateGuildThread(jsonChannel, client),
            ChannelType.StageGuildChannel => new StageGuildChannel(jsonChannel, jsonChannel.GuildId.GetValueOrDefault(), client),
            ChannelType.DirectoryGuildChannel => new DirectoryGuildChannel(jsonChannel, jsonChannel.GuildId.GetValueOrDefault(), client),
            ChannelType.ForumGuildChannel => new ForumGuildChannel(jsonChannel, jsonChannel.GuildId.GetValueOrDefault(), client),
            ChannelType.MediaForumGuildChannel => new MediaForumGuildChannel(jsonChannel, jsonChannel.GuildId.GetValueOrDefault(), client),
            _ => new UnknownChannel(jsonChannel, client),
        };
    }
}
