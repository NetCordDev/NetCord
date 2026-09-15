using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a generic Discord channel.
/// </summary>
public abstract partial class Channel(JsonChannel jsonModel, RestClient client) : ClientEntity(client), IJsonModel<JsonChannel>, IInteractionChannel
{
    JsonChannel IJsonModel<JsonChannel>.JsonModel => jsonModel;

    /// <summary>
    /// The unique identifier for this channel.
    /// </summary>
    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// The applied flags on the channel.
    /// </summary>
    public ChannelFlags Flags => jsonModel.Flags.GetValueOrDefault();

    Permissions IInteractionChannel.Permissions => jsonModel.Permissions.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) 
        => Mention.TryFormatChannel(destination, out charsWritten, Id);

    public static Channel CreateFromJson(JsonChannel jsonChannel, RestClient client)
    {
        var guildId = jsonChannel.GuildId.GetValueOrDefault();

        return jsonChannel.Type switch
        {
            ChannelType.TextGuildChannel          => new TextGuildChannel(jsonChannel, guildId, client),
            ChannelType.DMChannel                 => new DMChannel(jsonChannel, client),
            ChannelType.VoiceGuildChannel         => new VoiceGuildChannel(jsonChannel, guildId, client),
            ChannelType.GroupDMChannel            => new GroupDMChannel(jsonChannel, client),
            ChannelType.CategoryChannel           => new CategoryGuildChannel(jsonChannel, guildId, client),
            ChannelType.AnnouncementGuildChannel  => new AnnouncementGuildChannel(jsonChannel, guildId, client),
            ChannelType.AnnouncementGuildThread   => new AnnouncementGuildThread(jsonChannel, client),
            ChannelType.PublicGuildThread         => new PublicGuildThread(jsonChannel, client),
            ChannelType.PrivateGuildThread        => new PrivateGuildThread(jsonChannel, client),
            ChannelType.StageGuildChannel         => new StageGuildChannel(jsonChannel, guildId, client),
            ChannelType.DirectoryGuildChannel     => new DirectoryGuildChannel(jsonChannel, guildId, client),
            ChannelType.ForumGuildChannel         => new ForumGuildChannel(jsonChannel, guildId, client),
            ChannelType.MediaForumGuildChannel     => new MediaForumGuildChannel(jsonChannel, guildId, client),
            _                                     => new UnknownChannel(jsonChannel, client),
        };
    }
}
