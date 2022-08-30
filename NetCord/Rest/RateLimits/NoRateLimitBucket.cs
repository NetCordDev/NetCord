using System.Text.Json;

using NetCord.Rest.HttpClients;

namespace NetCord.Rest.RateLimits;

internal class NoRateLimitBucket : IBucket
{
    private protected readonly RestClient _client;

    public virtual async Task<HttpResponseMessage> SendAsync(IHttpClient client, Func<HttpRequestMessage> message, RequestProperties? properties)
    {
        var response = await client.SendAsync(message()).ConfigureAwait(false);

        if (response.IsSuccessStatusCode)
            return response;
        else
            throw new RestException(response);
    }

    private protected static long GetNewGlobalReset(HttpResponseMessage response) => DateTimeOffset.UtcNow.AddSeconds(JsonDocument.Parse(response.Content.ReadAsStream()).RootElement.GetProperty("retry_after").GetDouble()).ToUnixTimeMilliseconds();

    private protected static long ParseReset(string reset)
    {
        long result = 0;
        int point = reset.Length - 4;
        for (int i = 0; i < point; i++)
        {
            result *= 10;
            result += reset[i] - '0';
        }
        result *= 10;
        result += reset[^3] - '0';
        result *= 10;
        result += reset[^2] - '0';
        result *= 10;
        result += reset[^1] - '0';
        return result;
    }

    public NoRateLimitBucket(RestClient client)
    {
        _client = client;
    }
}
