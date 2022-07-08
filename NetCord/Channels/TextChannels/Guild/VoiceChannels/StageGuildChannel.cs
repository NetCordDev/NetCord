using NetCord.Rest;

namespace NetCord;

public class StageGuildChannel : TextChannel, IVoiceGuildChannel
{
    public int Bitrate => (int)_jsonModel.Bitrate!;
    public Snowflake? CategoryId => _jsonModel.ParentId;
    public string? Topic => _jsonModel.Topic;
    public string RtcRegion => _jsonModel.RtcRegion;
    public VideoQualityMode VideoQualityMode => VideoQualityMode.None;

    public string Name => _jsonModel.Name!;

    public int Position => (int)_jsonModel.Position!;

    public IReadOnlyDictionary<Snowflake, PermissionOverwrite> PermissionOverwrites { get; }

    public StageGuildChannel(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
        PermissionOverwrites = jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
    }

    public async Task<IGuildChannel> ModifyAsync(Action<GuildChannelOptions> action, RequestProperties? properties = null) => (IGuildChannel)await _client.ModifyGuildChannelAsync(Id, action, properties).ConfigureAwait(false);
}