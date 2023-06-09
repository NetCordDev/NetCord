using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<string> GetGatewayAsync(RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/gateway", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonGateway.JsonGatewaySerializerContext.WithOptions.JsonGateway).ConfigureAwait(false)).Url;

    public async Task<GatewayBot> GetGatewayBotAsync(RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/gateway/bot", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonGatewayBot.JsonGatewayBotSerializerContext.WithOptions.JsonGatewayBot).ConfigureAwait(false));
}
