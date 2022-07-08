namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<StageInstance> CreateStageInstanceAsync(StageInstanceProperties stageInstanceProperties, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, "/stage-instances", new(RateLimits.RouteParameter.CreateStageInstance), new JsonContent(stageInstanceProperties), properties).ConfigureAwait(false)).ToObject<JsonModels.JsonStageInstance>(), this);

    public async Task<StageInstance> GetStageInstanceAsync(Snowflake channelId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/stage-instances/{channelId}", properties).ConfigureAwait(false)).ToObject<JsonModels.JsonStageInstance>(), this);

    public async Task<StageInstance> ModifyStageInstanceAsync(Snowflake channelId, Action<StageInstanceOptions> action, RequestProperties? properties = null)
    {
        StageInstanceOptions stageInstanceOptions = new();
        action(stageInstanceOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, $"/stage-instances/{channelId}", new(RateLimits.RouteParameter.ModifyStageInstance), new JsonContent(stageInstanceOptions), properties).ConfigureAwait(false)).ToObject<JsonModels.JsonStageInstance>(), this);
    }

    public Task DeleteStageInstanceAsync(Snowflake channelId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/stage-instances/{channelId}", new RateLimits.Route(RateLimits.RouteParameter.DeleteStageInstance), properties);
}