﻿namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IEnumerable<VoiceRegion>> GetVoiceRegionsAsync(RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/voice/regions", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonVoiceRegionArray).ConfigureAwait(false)).Select(r => new VoiceRegion(r));
}
