namespace NetCord.Rest;

public partial class RestClient
{
    [GenerateAlias([typeof(CurrentApplication)], CastType = typeof(Application), Modifiers = ["override"])]
    public async Task<CurrentApplication> GetCurrentApplicationAsync(RestRequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/applications/@me", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplication).ConfigureAwait(false), this);

    [GenerateAlias([typeof(CurrentApplication)])]
    public async Task<CurrentApplication> ModifyCurrentApplicationAsync(Action<CurrentApplicationOptions> action, RestRequestProperties? properties = null)
    {
        CurrentApplicationOptions currentApplicationOptions = new();
        action(currentApplicationOptions);
        using (HttpContent content = new JsonContent<CurrentApplicationOptions>(currentApplicationOptions, Serialization.Default.CurrentApplicationOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/applications/@me", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplication).ConfigureAwait(false), this);
    }
}
