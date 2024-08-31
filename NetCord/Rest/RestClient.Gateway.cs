﻿namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<string> GetGatewayAsync(RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/gateway", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGateway).ConfigureAwait(false)).Url;

    public async Task<GatewayBot> GetGatewayBotAsync(RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/gateway/bot", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGatewayBot).ConfigureAwait(false));
}
