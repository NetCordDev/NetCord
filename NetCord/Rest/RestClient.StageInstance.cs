namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<StageInstance> CreateStageInstanceAsync(StageInstanceProperties stageInstanceProperties, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, "/stage-instances", new(RateLimits.RouteParameter.CreateStageInstance), new JsonContent<StageInstanceProperties>(stageInstanceProperties, StageInstanceProperties.StageInstancePropertiesSerializerContext.WithOptions.StageInstanceProperties), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonStageInstance.JsonStageInstanceSerializerContext.WithOptions.JsonStageInstance), this);

    public async Task<StageInstance> GetStageInstanceAsync(ulong channelId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/stage-instances/{channelId}", properties).ConfigureAwait(false)).ToObject(JsonModels.JsonStageInstance.JsonStageInstanceSerializerContext.WithOptions.JsonStageInstance), this);

    public async Task<StageInstance> ModifyStageInstanceAsync(ulong channelId, Action<StageInstanceOptions> action, RequestProperties? properties = null)
    {
        StageInstanceOptions stageInstanceOptions = new();
        action(stageInstanceOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, $"/stage-instances/{channelId}", new(RateLimits.RouteParameter.ModifyStageInstance), new JsonContent<StageInstanceOptions>(stageInstanceOptions, StageInstanceOptions.StageInstanceOptionsSerializerContext.WithOptions.StageInstanceOptions), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonStageInstance.JsonStageInstanceSerializerContext.WithOptions.JsonStageInstance), this);
    }

    public Task DeleteStageInstanceAsync(ulong channelId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/stage-instances/{channelId}", new RateLimits.Route(RateLimits.RouteParameter.DeleteStageInstance), properties);
}
