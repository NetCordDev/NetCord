using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;

using NetCord.Rest.HttpClients;
using NetCord.Rest.RateLimits;

namespace NetCord.Rest;

public partial class RestClient : IDisposable
{
    private readonly string _baseUrl;
    private readonly IHttpClient _httpClient;
    private readonly RequestProperties _defaultRequestProperties;

    private readonly object _rateLimitersLock = new();
    private readonly GlobalRateLimiter _globalRateLimiter = new();
    private readonly Dictionary<BucketInfo, RouteRateLimiter> _rateLimitersFromBuckets = [];
    private readonly Dictionary<Route, RouteRateLimiter> _rateLimiters = [];
    private readonly Dictionary<Route, SemaphoreSlim?> _knownRoutesWithoutRateLimiters = [];

    public RestClient(RestClientConfiguration? configuration = null)
    {
        configuration ??= new();

        _baseUrl = $"https://{configuration.Hostname ?? Discord.RestHostname}/api/v{(int)configuration.Version}";

        var httpClient = configuration.HttpClient ?? new HttpClients.HttpClient();
        httpClient.AddDefaultRequestHeader("User-Agent", UserAgentHeader);
        _httpClient = httpClient;

        _defaultRequestProperties = configuration.DefaultRequestProperties ?? new();
    }

    public RestClient(Token token, RestClientConfiguration? configuration = null)
    {
        configuration ??= new();

        _baseUrl = $"https://{configuration.Hostname ?? Discord.RestHostname}/api/v{(int)configuration.Version}";

        var httpClient = configuration.HttpClient ?? new HttpClients.HttpClient();
        httpClient.AddDefaultRequestHeader("User-Agent", UserAgentHeader);
        httpClient.AddDefaultRequestHeader("Authorization", token.ToHttpHeader());
        _httpClient = httpClient;

        _defaultRequestProperties = configuration.DefaultRequestProperties ?? new();
    }

    public async Task<Stream> SendRequestAsync(HttpMethod method, FormattableString endpoint, string? query = null, TopLevelResourceInfo? resourceInfo = null, RequestProperties? properties = null, bool global = true)
    {
        properties ??= _defaultRequestProperties;
        string url = $"{_baseUrl}{endpoint}{query}";

        var response = await SendRequestAsync(new(method, endpoint.Format, resourceInfo), global, () =>
        {
            HttpRequestMessage requestMessage = new(method, url);
            properties.AddHeaders(requestMessage.Headers);
            return requestMessage;
        }, properties).ConfigureAwait(false);

        return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> SendRequestAsync(HttpMethod method, HttpContent content, FormattableString endpoint, string? query = null, TopLevelResourceInfo? resourceInfo = null, RequestProperties? properties = null, bool global = true)
    {
        properties ??= _defaultRequestProperties;
        string url = $"{_baseUrl}{endpoint}{query}";

        var response = await SendRequestAsync(new(method, endpoint.Format, resourceInfo), global, () =>
        {
            HttpRequestMessage requestMessage = new(method, url)
            {
                Content = content,
            };
            properties.AddHeaders(requestMessage.Headers);
            return requestMessage;
        }, properties).ConfigureAwait(false);

        return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    private async Task<HttpResponseMessage> SendRequestAsync(Route route, bool global, Func<HttpRequestMessage> messageFunc, RequestProperties properties)
    {
        while (true)
        {
            if (global)
                await AcquireRateLimiter(_globalRateLimiter, true, properties).ConfigureAwait(false);

            bool rateLimiterFound;
            RouteRateLimiter? rateLimiter;

            bool isNotFirstUse;
            SemaphoreSlim? semaphore;

            lock (_rateLimitersLock)
            {
                if (rateLimiterFound = _rateLimiters.TryGetValue(route, out rateLimiter))
                {
                    isNotFirstUse = true;
                    semaphore = null;
                }
                else if (!(isNotFirstUse = _knownRoutesWithoutRateLimiters.TryGetValue(route, out semaphore)))
                    _knownRoutesWithoutRateLimiters.Add(route, semaphore = new(0));
            }

            if (rateLimiterFound)
                await AcquireRateLimiter(rateLimiter!, false, properties).ConfigureAwait(false);
            else if (isNotFirstUse && semaphore is not null)
            {
                await semaphore.WaitAsync().ConfigureAwait(false);
                continue;
            }

            var message = messageFunc();
            var timestamp = Environment.TickCount64;
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(message).ConfigureAwait(false);
            }
            catch
            {
                semaphore?.Release();
                throw;
            }

            var headers = response.Headers;
            long reset = 0;
            bool bucketFound;
            if (bucketFound = TryGetBucketHeaderValue(headers, out var bucket))
            {
                if (rateLimiterFound)
                    HandleResponse(route, bucket!, headers, timestamp, rateLimiter!, ref reset);
                else
                    HandleResponseWithoutRateLimiter(route, bucket!, headers, timestamp, ref reset, semaphore);
            }

            if (IsRateLimited(headers, out var scope))
            {
                if (properties.RateLimitHandling.HasFlag((RateLimitHandling)scope))
                {
                    if (scope is RateLimitScope.Global)
                    {
                        _globalRateLimiter.RateLimitReceived(timestamp + (int)headers.RetryAfter!.Delta.GetValueOrDefault().TotalMilliseconds);
                        if (!bucketFound)
                            semaphore?.Release();
                    }
                    else
                    {
                        if (!bucketFound)
                            SetRouteWithoutRateLimit(route, semaphore);

                        if (scope is RateLimitScope.Shared)
                            await Task.Delay(headers.RetryAfter!.Delta.GetValueOrDefault()).ConfigureAwait(false);
                    }
                    continue;
                }
                else
                {
                    if (scope is RateLimitScope.Global)
                    {
                        _globalRateLimiter.RateLimitReceived(timestamp + (int)headers.RetryAfter!.Delta.GetValueOrDefault().TotalMilliseconds);
                        if (!bucketFound)
                            semaphore?.Release();
                    }
                    else if (!bucketFound)
                        SetRouteWithoutRateLimit(route, semaphore);

                    throw new RateLimitedException(reset, scope);
                }
            }
            else if (!bucketFound)
                SetRouteWithoutRateLimit(route, semaphore);

            if (response.IsSuccessStatusCode)
                return response;

            RestError error;
            var content = response.Content;
            if (content.Headers.ContentType is { MediaType: "application/json" })
            {
                try
                {
                    error = (await JsonSerializer.DeserializeAsync(await content.ReadAsStreamAsync().ConfigureAwait(false), Serialization.Default.RestError).ConfigureAwait(false))!;
                }
                catch
                {
                    throw new RestException(response.StatusCode, response.ReasonPhrase!);
                }
            }
            else
                throw new RestException(response.StatusCode, response.ReasonPhrase!);

            throw new RestException(response.StatusCode, response.ReasonPhrase!, error);
        }
    }

    private void HandleResponse(Route route, string bucket, HttpResponseHeaders headers, long timestamp, RouteRateLimiter rateLimiter, ref long reset)
    {
        var currentBucket = rateLimiter.BucketInfo.Bucket;
        if (bucket != currentBucket)
        {
            bool update;
            RouteRateLimiter? newRateLimiter;
            BucketInfo bucketInfo = new(bucket, route.ResourceInfo);
            lock (_rateLimitersLock)
            {
                if (update = _rateLimitersFromBuckets.TryGetValue(bucketInfo, out newRateLimiter))
                    _rateLimiters[route] = newRateLimiter!;
                else if (TryGetLimitHeaderValue(headers, out var limit) && TryGetRemainingHeaderValue(headers, out var remaining) && TryGetResetAfterHeaderValue(headers, out var resetAfter))
                {
                    newRateLimiter = new(limit, remaining, reset = timestamp + resetAfter, resetAfter, bucketInfo);
                    _rateLimiters[route] = newRateLimiter;
                    _rateLimitersFromBuckets.Add(bucketInfo, newRateLimiter);
                }
            }

            if (update)
            {
                if (TryGetRemainingHeaderValue(headers, out var remaining) && TryGetResetAfterHeaderValue(headers, out var resetAfter))
                    newRateLimiter!.Update(remaining, reset = timestamp + resetAfter, resetAfter);
            }

            rateLimiter.CancelAcquire(timestamp);
        }
        else if (TryGetRemainingHeaderValue(headers, out var remaining) && TryGetResetAfterHeaderValue(headers, out var resetAfter))
            rateLimiter.Update(remaining, reset = timestamp + resetAfter, resetAfter);
    }

    private void HandleResponseWithoutRateLimiter(Route route, string bucket, HttpResponseHeaders headers, long timestamp, ref long reset, SemaphoreSlim? semaphore)
    {
        bool update;
        RouteRateLimiter? rateLimiter;
        bool removeSemaphore;
        BucketInfo bucketInfo = new(bucket, route.ResourceInfo);
        lock (_rateLimitersLock)
        {
            if (update = _rateLimitersFromBuckets.TryGetValue(bucketInfo, out rateLimiter))
            {
                _rateLimiters.TryAdd(route, rateLimiter!);
                removeSemaphore = true;
            }
            else if (TryGetLimitHeaderValue(headers, out var limit) && TryGetRemainingHeaderValue(headers, out var remaining) && TryGetResetAfterHeaderValue(headers, out var resetAfter))
            {
                rateLimiter = new(limit, remaining, reset = timestamp + resetAfter, resetAfter, bucketInfo);
                _rateLimiters.Add(route, rateLimiter);
                _rateLimitersFromBuckets.Add(bucketInfo, rateLimiter);
                removeSemaphore = true;
            }
            else
                removeSemaphore = false;
        }

        if (update)
        {
            if (TryGetRemainingHeaderValue(headers, out var remaining) && TryGetResetAfterHeaderValue(headers, out var resetAfter))
                rateLimiter!.Update(remaining, reset = timestamp + resetAfter, resetAfter);
        }

        if (removeSemaphore)
            RemoveSemaphore(route, semaphore);
        else
            semaphore?.Release();
    }

    private void RemoveSemaphore(Route route, SemaphoreSlim? semaphore)
    {
        if (semaphore is not null)
        {
            lock (_rateLimitersLock)
                _knownRoutesWithoutRateLimiters.Remove(route);
            semaphore.Release(int.MaxValue);
            semaphore.Dispose();
        }
    }

    private void SetRouteWithoutRateLimit(Route route, SemaphoreSlim? semaphore)
    {
        if (semaphore is not null)
        {
            lock (_rateLimitersLock)
                _knownRoutesWithoutRateLimiters[route] = null;
            semaphore.Release(int.MaxValue);
            semaphore.Dispose();
        }
    }

    private static async Task AcquireRateLimiter(IRateLimiter rateLimiter, bool global, RequestProperties properties)
    {
        while (true)
        {
            var info = rateLimiter.TryAcquire();
            if (info.RateLimited)
            {
                if (properties.RateLimitHandling.HasFlag(global ? RateLimitHandling.RetryGlobal : RateLimitHandling.RetryUser))
                    await Task.Delay(info.ResetAfter).ConfigureAwait(false);
                else
                    throw new RateLimitedException(Environment.TickCount64 + info.ResetAfter, global ? RateLimitScope.Global : RateLimitScope.User);
            }
            else
                break;
        }
    }

    private static bool TryGetBucketHeaderValue(HttpResponseHeaders headers, [MaybeNullWhen(false)] out string bucket)
    {
        if (headers.TryGetValues("x-ratelimit-bucket", out var values))
        {
            bucket = values.First();
            return true;
        }

        bucket = null;
        return false;
    }

    private static bool TryGetLimitHeaderValue(HttpResponseHeaders headers, out int limit)
    {
        if (headers.TryGetValues("x-ratelimit-limit", out var values))
        {
            limit = int.Parse(values.First(), CultureInfo.InvariantCulture);
            return true;
        }

        limit = 0;
        return false;
    }

    private static bool TryGetRemainingHeaderValue(HttpResponseHeaders headers, out int remaining)
    {
        if (headers.TryGetValues("x-ratelimit-remaining", out var values))
        {
            remaining = int.Parse(values.First(), CultureInfo.InvariantCulture);
            return true;
        }

        remaining = 0;
        return false;
    }

    private static bool TryGetResetAfterHeaderValue(HttpResponseHeaders headers, out int resetAfter)
    {
        if (headers.TryGetValues("x-ratelimit-reset-after", out var values))
        {
            resetAfter = (int)(float.Parse(values.First(), CultureInfo.InvariantCulture) * 1000);
            return true;
        }

        resetAfter = 0;
        return false;
    }

    private static bool IsRateLimited(HttpResponseHeaders headers, out RateLimitScope scope)
    {
        if (headers.TryGetValues("x-ratelimit-scope", out var values))
        {
            scope = values.First() switch
            {
                "user" => RateLimitScope.User,
                "global" => RateLimitScope.Global,
                "shared" => RateLimitScope.Shared,
                _ => (RateLimitScope)(-1),
            };
            return true;
        }

        scope = default;
        return false;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
