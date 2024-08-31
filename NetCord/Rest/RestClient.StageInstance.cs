namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<StageInstance> CreateStageInstanceAsync(StageInstanceProperties stageInstanceProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<StageInstanceProperties>(stageInstanceProperties, Serialization.Default.StageInstanceProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/stage-instances", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonStageInstance).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(StageGuildChannel)], nameof(StageGuildChannel.Id))]
    [GenerateAlias([typeof(StageInstance)], nameof(StageInstance.ChannelId))]
    public async Task<StageInstance> GetStageInstanceAsync(ulong channelId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/stage-instances/{channelId}", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonStageInstance).ConfigureAwait(false), this);

    [GenerateAlias([typeof(StageGuildChannel)], nameof(StageGuildChannel.Id))]
    [GenerateAlias([typeof(StageInstance)], nameof(StageInstance.ChannelId))]
    public async Task<StageInstance> ModifyStageInstanceAsync(ulong channelId, Action<StageInstanceOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        StageInstanceOptions stageInstanceOptions = new();
        action(stageInstanceOptions);
        using (HttpContent content = new JsonContent<StageInstanceOptions>(stageInstanceOptions, Serialization.Default.StageInstanceOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/stage-instances/{channelId}", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonStageInstance).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(StageGuildChannel)], nameof(StageGuildChannel.Id))]
    [GenerateAlias([typeof(StageInstance)], nameof(StageInstance.ChannelId))]
    public Task DeleteStageInstanceAsync(ulong channelId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/stage-instances/{channelId}", null, null, properties);
}
