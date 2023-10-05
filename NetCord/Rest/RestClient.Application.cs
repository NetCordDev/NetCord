using NetCord.JsonModels;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<CurrentApplication> GetCurrentApplicationAsync(RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/applications/@me", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonApplication.JsonApplicationSerializerContext.WithOptions.JsonApplication).ConfigureAwait(false), this);

    public async Task<CurrentApplication> ModifyCurrentApplicationAsync(Action<CurrentApplicationOptions> action, RequestProperties? properties = null)
    {
        CurrentApplicationOptions currentApplicationOptions = new();
        action(currentApplicationOptions);
        using (HttpContent content = new JsonContent<CurrentApplicationOptions>(currentApplicationOptions, CurrentApplicationOptions.CurrentApplicationOptionsSerializerContext.WithOptions.CurrentApplicationOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/applications/@me", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonApplication.JsonApplicationSerializerContext.WithOptions.JsonApplication).ConfigureAwait(false), this);
    }
}
