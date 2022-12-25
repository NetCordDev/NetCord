using System.Text.Json.Serialization;

namespace NetCord.Rest.RateLimits;

internal partial class NoRateLimitBucket : IBucket
{
    private protected readonly RestClient _client;

    public virtual async Task<HttpResponseMessage> SendAsync(Func<HttpRequestMessage> message, RequestProperties? properties)
    {
        var response = await _client._httpClient.SendAsync(message()).ConfigureAwait(false);

        if (response.IsSuccessStatusCode)
            return response;
        else
            throw new RestException(response);
    }

    private protected static async Task<long> GetNewGlobalResetAsync(HttpResponseMessage response)
    {
        var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var retryAfter = (long)((await stream.ToObjectAsync(JsonGlobalRateLimit.JsonGlobalRateLimitSerializerContext.WithOptions.JsonGlobalRateLimit).ConfigureAwait(false)).RetryAfter * 1000);
        return Environment.TickCount64 + retryAfter;
    }

    private partial class JsonGlobalRateLimit
    {
        [JsonPropertyName("retry_after")]
        public float RetryAfter { get; set; }

        [JsonSerializable(typeof(JsonGlobalRateLimit))]
        public partial class JsonGlobalRateLimitSerializerContext : JsonSerializerContext
        {
            public static JsonGlobalRateLimitSerializerContext WithOptions { get; } = new(Serialization.Options);
        }
    }

    public NoRateLimitBucket(RestClient client)
    {
        _client = client;
    }
}
