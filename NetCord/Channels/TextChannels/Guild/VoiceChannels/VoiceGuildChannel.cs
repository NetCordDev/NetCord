using NetCord.Rest;

namespace NetCord;

public class VoiceGuildChannel : TextChannel, IVoiceGuildChannel
{
    public int Bitrate => (int)_jsonModel.Bitrate!;
    public Snowflake? CategoryId => _jsonModel.ParentId;
    public int UserLimit => (int)_jsonModel.UserLimit!; //
    public string RtcRegion => _jsonModel.RtcRegion;
    public VideoQualityMode VideoQualityMode
        => _jsonModel.VideoQualityMode != null ? (VideoQualityMode)_jsonModel.VideoQualityMode : VideoQualityMode.Auto;

    public string Name => _jsonModel.Name!;

    public int Position => (int)_jsonModel.Position!;

    public IReadOnlyDictionary<Snowflake, PermissionOverwrite> PermissionOverwrites { get; }

    public VoiceGuildChannel(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
        PermissionOverwrites = jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
    }

    public async Task<IGuildChannel> ModifyAsync(Action<GuildChannelOptions> action, RequestProperties? options = null) => (IGuildChannel)await _client.ModifyGuildChannelAsync(Id, action, options).ConfigureAwait(false);
}