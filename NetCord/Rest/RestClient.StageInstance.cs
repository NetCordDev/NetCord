namespace NetCord;

public partial class RestClient
{
    public async Task<StageInstance> CreateStageInstanceAsync(StageInstanceProperties properties, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, new JsonContent(properties), "/stage-instances", options).ConfigureAwait(false)).ToObject<JsonModels.JsonStageInstance>(), this);

    public async Task<StageInstance> GetStageInstanceAsync(Snowflake channelId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/stage-instances/{channelId}", options).ConfigureAwait(false)).ToObject<JsonModels.JsonStageInstance>(), this);

    public async Task<StageInstance> ModifyStageInstanceAsync(Snowflake channelId, Action<StageInstanceOptions> action, RequestProperties? options = null)
    {
        StageInstanceOptions stageInstanceOptions = new();
        action(stageInstanceOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(stageInstanceOptions), $"/stage-instances/{channelId}", options).ConfigureAwait(false)).ToObject<JsonModels.JsonStageInstance>(), this);
    }

    public Task DeleteStageInstanceAsync(Snowflake channelId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/stage-instances/{channelId}", options);
}
