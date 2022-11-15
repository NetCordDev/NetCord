using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;

namespace NetCord.Rest.RateLimits;

internal class GlobalBucket : NoRateLimitBucket
{
    private readonly AdjustableSemaphoreSlim _semaphore;

    public override async Task<HttpResponseMessage> SendAsync(Func<HttpRequestMessage> message, RequestProperties? properties)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            if (properties != null && properties.RateLimitHandling == RateLimitHandling.NoRetry)
            {
                int now = Environment.TickCount;
                EnsureNoGlobalRateLimit(now, _client);

                var response = await _client._httpClient.SendAsync(message()).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                    return response;
                else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests && HasGlobalRateLimit(response.Headers))
                {
                    await UpdateGlobalRateLimitDataAndThrowAsync(response, _client).ConfigureAwait(false);
                    throw null;
                }
                else
                    throw new RestException(response);
            }
            else
            {
                while (true)
                {
                    var now = Environment.TickCount;
                    var globalReset = _client._globalRateLimitReset;
                    if (HasGlobalRateLimit(now, globalReset, out int diff))
                        await WaitForGlobalRateLimitEndAsync(diff).ConfigureAwait(false);

                    var response = await _client._httpClient.SendAsync(message()).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                        return response;
                    else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        if (HasGlobalRateLimit(response.Headers))
                            await UpdateGlobalRateLimitDataAsync(response, _client).ConfigureAwait(false);
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

    public GlobalBucket(RestClient client) : base(client)
    {
        _semaphore = new(50);
    }

    internal static bool HasGlobalRateLimit(HttpHeaders headers) => headers.Contains("x-ratelimit-global");

    internal static void EnsureNoGlobalRateLimit(int now, RestClient client)
    {
        int globalReset = client._globalRateLimitReset;
        int diff = globalReset - now;
        if (diff > 0)
            throw new RateLimitedException(globalReset, true);
    }

    internal static bool HasGlobalRateLimit(int now, int globalReset, out int diff)
    {
        diff = globalReset - now;
        return diff > 0;
    }

    internal static Task WaitForGlobalRateLimitEndAsync(int diff) => Task.Delay(diff);

    [DoesNotReturn]
    internal static async Task UpdateGlobalRateLimitDataAndThrowAsync(HttpResponseMessage response, RestClient client)
    {
        int newGlobalReset = await GetNewGlobalResetAsync(response).ConfigureAwait(false);
        int diff = client._globalRateLimitReset - newGlobalReset;
        throw new RateLimitedException(diff >= 0 ? client._globalRateLimitReset : client._globalRateLimitReset = newGlobalReset, true);
    }

    internal static async Task UpdateGlobalRateLimitDataAsync(HttpResponseMessage response, RestClient client)
    {
        int newGlobalReset = await GetNewGlobalResetAsync(response).ConfigureAwait(false);
        int diff = client._globalRateLimitReset - newGlobalReset;
        if (diff >= 0)
            client._globalRateLimitReset = newGlobalReset;
    }
}
