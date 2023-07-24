using NetCord.JsonModels;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<Application> GetCurrentApplicationAsync(RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/applications/@me", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonApplication.JsonApplicationSerializerContext.WithOptions.JsonApplication).ConfigureAwait(false), this);
}
