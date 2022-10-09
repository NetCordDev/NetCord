namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<string> GetGatewayAsync(RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, "/gateway", properties).ConfigureAwait(false)).ToObject(JsonModels.JsonGateway.JsonGatewaySerializerContext.WithOptions.JsonGateway).Url;

    public async Task<GatewayBot> GetGatewayBotAsync(RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, "/gateway/bot", new RateLimits.Route(RateLimits.RouteParameter.GetGatewayBot), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonGatewayBot.JsonGatewayBotSerializerContext.WithOptions.JsonGatewayBot));
}
