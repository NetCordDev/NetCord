namespace NetCord;

public partial class RestClient
{
    public async Task<IEnumerable<VoiceRegion>> GetVoiceRegionsAsync(RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, "/voice/regions", options).ConfigureAwait(false)).ToObject<JsonModels.JsonVoiceRegion[]>().Select(r => new VoiceRegion(r));
}