namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<CurrentApplication> GetCurrentBotApplicationInformationAsync(RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/oauth2/applications/@me", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplication).ConfigureAwait(false), this);

    public async Task<AuthorizationInformation> GetCurrentAuthorizationInformationAsync(RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/oauth2/@me", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonAuthorizationInformation).ConfigureAwait(false), this);
}
