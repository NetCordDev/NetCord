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

    private protected static async Task<int> GetNewGlobalResetAsync(HttpResponseMessage response)
    {
        var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var retryAfter = (int)(stream.ToObject(JsonGlobalRateLimit.JsonGlobalRateLimitSerializerContext.WithOptions.JsonGlobalRateLimit).RetryAfter * 1000);
        return Environment.TickCount + retryAfter;
    }

    private partial class JsonGlobalRateLimit
    {
        [JsonPropertyName("retry_after")]
        public float RetryAfter { get; set; }

        [JsonSerializable(typeof(JsonGlobalRateLimit))]
        public partial class JsonGlobalRateLimitSerializerContext : JsonSerializerContext
        {
            public static JsonGlobalRateLimitSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
        }
    }

    private protected static int GetReset(string resetAfter)
    {
        int result = 0;
        int point = resetAfter.Length - 4;
        for (int i = 0; i < point; i++)
        {
            result *= 10;
            result += resetAfter[i] - '0';
        }

        result *= 10;
        result += resetAfter[^3] - '0';
        result *= 10;
        result += resetAfter[^2] - '0';
        result *= 10;
        result += resetAfter[^1] - '0';
        return Environment.TickCount + result;
    }

    public NoRateLimitBucket(RestClient client)
    {
        _client = client;
    }
}
