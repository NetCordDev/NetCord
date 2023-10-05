using NetCord.JsonModels;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<Application> GetCurrentApplicationAsync(RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/applications/@me", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonApplication.JsonApplicationSerializerContext.WithOptions.JsonApplication).ConfigureAwait(false), this);

    public async Task<Application> ModifyCurrentApplicationAsync(Action<ApplicationOptions> action, RequestProperties? properties = null)
    {
        ApplicationOptions applicationOptions = new();
        action(applicationOptions);
        using (HttpContent content = new JsonContent<ApplicationOptions>(applicationOptions, ApplicationOptions.ApplicationOptionsSerializerContext.WithOptions.ApplicationOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/applications/@me", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonApplication.JsonApplicationSerializerContext.WithOptions.JsonApplication).ConfigureAwait(false), this);
    }
}
