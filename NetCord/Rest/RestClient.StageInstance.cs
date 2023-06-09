using NetCord.JsonModels;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<StageInstance> CreateStageInstanceAsync(StageInstanceProperties stageInstanceProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<StageInstanceProperties>(stageInstanceProperties, StageInstanceProperties.StageInstancePropertiesSerializerContext.WithOptions.StageInstanceProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/stage-instances", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonStageInstance.JsonStageInstanceSerializerContext.WithOptions.JsonStageInstance).ConfigureAwait(false), this);
    }

    public async Task<StageInstance> GetStageInstanceAsync(ulong channelId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/stage-instances/{channelId}", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonStageInstance.JsonStageInstanceSerializerContext.WithOptions.JsonStageInstance).ConfigureAwait(false), this);

    public async Task<StageInstance> ModifyStageInstanceAsync(ulong channelId, Action<StageInstanceOptions> action, RequestProperties? properties = null)
    {
        StageInstanceOptions stageInstanceOptions = new();
        action(stageInstanceOptions);
        using (HttpContent content = new JsonContent<StageInstanceOptions>(stageInstanceOptions, StageInstanceOptions.StageInstanceOptionsSerializerContext.WithOptions.StageInstanceOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/stage-instances/{channelId}", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonStageInstance.JsonStageInstanceSerializerContext.WithOptions.JsonStageInstance).ConfigureAwait(false), this);
    }

    public Task DeleteStageInstanceAsync(ulong channelId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/stage-instances/{channelId}", null, null, properties);
}
