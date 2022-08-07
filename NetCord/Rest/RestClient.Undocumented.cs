namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<Application> GetApplicationAsync(Snowflake applicationId, RequestProperties? properties = null) => new((await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/rpc", properties).ConfigureAwait(false)).ToObject<JsonModels.JsonApplication>(), this);
}