namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IEnumerable<VoiceRegion>> GetVoiceRegionsAsync(RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, "/voice/regions", properties).ConfigureAwait(false)).ToObject(JsonModels.JsonVoiceRegion.JsonVoiceRegionArraySerializerContext.WithOptions.JsonVoiceRegionArray).Select(r => new VoiceRegion(r));
}
