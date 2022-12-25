using NetCord.JsonModels;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<Application> GetCurrentBotApplicationInformationAsync(RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, "/oauth2/applications/@me", properties).ConfigureAwait(false)).ToObjectAsync(JsonApplication.JsonApplicationSerializerContext.WithOptions.JsonApplication).ConfigureAwait(false), this);

    public async Task<AuthorizationInformation> GetCurrentAuthorizationInformationAsync(RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, "/oauth2/@me", properties).ConfigureAwait(false)).ToObjectAsync(JsonAuthorizationInformation.JsonAuthorizationInformationSerializerContext.WithOptions.JsonAuthorizationInformation).ConfigureAwait(false), this);
}
