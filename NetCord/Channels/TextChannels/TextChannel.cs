using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public abstract partial class TextChannel : Channel
{
    private protected TextChannel(JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
    }

    public ulong? LastMessageId => _jsonModel.LastMessageId;
    public DateTimeOffset? LastPin => _jsonModel.LastPin;

    public static new TextChannel CreateFromJson(JsonChannel jsonChannel, RestClient client)
    {
        return jsonChannel.Type switch
        {
            ChannelType.TextGuildChannel => new TextGuildChannel(jsonChannel, jsonChannel.GuildId.GetValueOrDefault(), client),
            ChannelType.DMChannel => new DMChannel(jsonChannel, client),
            ChannelType.VoiceGuildChannel => new VoiceGuildChannel(jsonChannel, jsonChannel.GuildId.GetValueOrDefault(), client),
            ChannelType.GroupDMChannel => new GroupDMChannel(jsonChannel, client),
            ChannelType.AnnouncementGuildChannel => new AnnouncementGuildChannel(jsonChannel, jsonChannel.GuildId.GetValueOrDefault(), client),
            ChannelType.AnnouncementGuildThread => new AnnouncementGuildThread(jsonChannel, client),
            ChannelType.PublicGuildThread => new PublicGuildThread(jsonChannel, client),
            ChannelType.PrivateGuildThread => new PrivateGuildThread(jsonChannel, client),
            ChannelType.StageGuildChannel => new StageGuildChannel(jsonChannel, jsonChannel.GuildId.GetValueOrDefault(), client),
            ChannelType.DirectoryGuildChannel => new DirectoryGuildChannel(jsonChannel, jsonChannel.GuildId.GetValueOrDefault(), client),
            _ => new UnknownTextChannel(jsonChannel, client),
        };
    }
}
