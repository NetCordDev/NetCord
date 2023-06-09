using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IEnumerable<VoiceRegion>> GetVoiceRegionsAsync(RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/voice/regions", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonVoiceRegion.JsonVoiceRegionArraySerializerContext.WithOptions.JsonVoiceRegionArray).ConfigureAwait(false)).Select(r => new VoiceRegion(r));
}
