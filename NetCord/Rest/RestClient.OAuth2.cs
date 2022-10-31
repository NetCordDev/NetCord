using NetCord.JsonModels;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<Application> GetCurrentBotApplicationInformationAsync(RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, "/oauth2/applications/@me", properties).ConfigureAwait(false)).ToObject(JsonApplication.JsonApplicationSerializerContext.WithOptions.JsonApplication), this);

    public async Task<AuthorizationInformation> GetCurrentAuthorizationInformationAsync(RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, "/oauth2/@me", properties).ConfigureAwait(false)).ToObject(JsonAuthorizationInformation.JsonAuthorizationInformationSerializerContext.WithOptions.JsonAuthorizationInformation), this);
}
