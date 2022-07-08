using System.Text.Json;

namespace NetCord.Rest.RateLimits;

internal class Bucket
{
    private protected readonly RestClient _client;

    private protected SemaphoreSlim _semaphore = new(50);

    public virtual async Task<HttpResponseMessage> SendAsync(HttpClient client, Func<HttpRequestMessage> message, RequestProperties? properties)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            if (properties != null && properties.RateLimitHandling == RateLimitHandling.NoRetry)
            {
                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var globalReset = _client._globalRateLimitReset;
                if (now < globalReset)
                    throw new RateLimitedException(globalReset, true);

                var response = await client.SendAsync(message()).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                    return response;
                else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    if (response.Headers.Contains("x-ratelimit-global"))
                    {
                        var newGlobalReset = GetNewGlobalReset(response);
                        if (_client._globalRateLimitReset < newGlobalReset)
                            throw new RateLimitedException(_client._globalRateLimitReset = newGlobalReset, true);
                        else
                            throw new RateLimitedException(_client._globalRateLimitReset, true);
                    }
                    else
                    {
                        if (response.Headers.TryGetValues("x-ratelimit-reset", out var values))
                            throw new RateLimitedException(ParseReset(values.First()), false);
                        else
                            throw new RestException(response);
                    }
                }
                else
                    throw new RestException(response);
            }
            else
            {
                while (true)
                {
                    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    var globalReset = _client._globalRateLimitReset;
                    if (now < globalReset)
                        await Task.Delay((int)(globalReset - now)).ConfigureAwait(false);

                    var response = await client.SendAsync(message()).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                        return response;
                    else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        if (response.Headers.Contains("x-ratelimit-global"))
                        {
                            var newGlobalReset = GetNewGlobalReset(response);
                            if (_client._globalRateLimitReset < newGlobalReset)
                                _client._globalRateLimitReset = newGlobalReset;
                        }
                    }
                    else
                        throw new RestException(response);
                }
            }
        }
        finally
        {
            _semaphore.Release();
        }
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

    public Bucket(RestClient client)
    {
        _client = client;
    }
}