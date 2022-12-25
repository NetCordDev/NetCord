namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IEnumerable<VoiceRegion>> GetVoiceRegionsAsync(RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, "/voice/regions", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonVoiceRegion.JsonVoiceRegionArraySerializerContext.WithOptions.JsonVoiceRegionArray).ConfigureAwait(false)).Select(r => new VoiceRegion(r));
}
