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
    private readonly IRateLimitManager _rateLimitManager;

    /// <summary>
    /// The token of the <see cref="RestClient"/>.
    /// </summary>
    public Token? Token { get; }

    public RestClient(RestClientConfiguration? configuration = null)
    {
        configuration ??= new();

        _baseUrl = $"https://{configuration.Hostname ?? Discord.RestHostname}/api/v{(int)configuration.Version}";

        var httpClient = configuration.HttpClient ?? new HttpClients.HttpClient();
        httpClient.AddDefaultRequestHeader("User-Agent", UserAgentHeader);
        _httpClient = httpClient;

        _defaultRequestProperties = configuration.DefaultRequestProperties ?? new();

        _rateLimitManager = configuration.RateLimitManager ?? new RateLimitManager();
    }

    public RestClient(Token token, RestClientConfiguration? configuration = null)
    {
        Token = token;
        configuration ??= new();

        _baseUrl = $"https://{configuration.Hostname ?? Discord.RestHostname}/api/v{(int)configuration.Version}";

        var httpClient = configuration.HttpClient ?? new HttpClients.HttpClient();
        httpClient.AddDefaultRequestHeader("User-Agent", UserAgentHeader);
        httpClient.AddDefaultRequestHeader("Authorization", token.ToHttpHeader());
        _httpClient = httpClient;

        _defaultRequestProperties = configuration.DefaultRequestProperties ?? new();

        _rateLimitManager = configuration.RateLimitManager ?? new RateLimitManager();
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
            var globalRateLimiter = global ? await AcquireGlobalRateLimiterAsync(properties).ConfigureAwait(false) : null;

            var rateLimiter = await AcquireRouteRateLimiterAsync(route, properties).ConfigureAwait(false);

            var message = messageFunc();
            var timestamp = Environment.TickCount64;
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(message).ConfigureAwait(false);
            }
            catch
            {
                await rateLimiter.CancelAcquireAsync(timestamp).ConfigureAwait(false);
                throw;
            }

            var headers = response.Headers;

            var rateLimited = IsRateLimited(headers, out var scope);

            if (TryGetBucketHeaderValue(headers, out var bucket)
                && TryGetRemainingHeaderValue(headers, out var remaining)
                && TryGetResetAfterHeaderValue(headers, out var resetAfter)
                && TryGetLimitHeaderValue(headers, out var limit))
            {
                BucketInfo bucketInfo = new(bucket, route.ResourceInfo);
                RateLimitInfo rateLimitInfo = new(timestamp, resetAfter, remaining, limit, bucketInfo);

                if (rateLimiter.HasBucketInfo)
                {
                    var rateLimiterBucketInfo = rateLimiter.BucketInfo;
                    if (rateLimiterBucketInfo == bucketInfo)
                        await rateLimiter.UpdateAsync(rateLimitInfo).ConfigureAwait(false);
                    else
                    {
                        await _rateLimitManager.ExchangeRouteRateLimiterAsync(route, rateLimitInfo, rateLimiterBucketInfo).ConfigureAwait(false);
                        await rateLimiter.IndicateExchangeAsync(timestamp).ConfigureAwait(false);
                    }
                }
                else
                {
                    await _rateLimitManager.ExchangeRouteRateLimiterAsync(route, rateLimitInfo, null).ConfigureAwait(false);
                    await rateLimiter.IndicateExchangeAsync(timestamp).ConfigureAwait(false);
                }

                if (rateLimited)
                {
                    if (properties.RateLimitHandling.HasFlag((RateLimitHandling)scope))
                    {
                        if (scope is RateLimitScope.Shared)
                            await Task.Delay(headers.RetryAfter!.Delta.GetValueOrDefault()).ConfigureAwait(false);

                        continue;
                    }

                    throw new RateLimitedException(timestamp + (long)headers.RetryAfter!.Delta.GetValueOrDefault().TotalMilliseconds, scope);
                }
            }
            else if (rateLimited)
            {
                if (scope is RateLimitScope.Global)
                {
                    if (global)
                        await globalRateLimiter!.IndicateRateLimitAsync(timestamp + (long)headers.RetryAfter!.Delta.GetValueOrDefault().TotalMilliseconds).ConfigureAwait(false);

                    await rateLimiter.CancelAcquireAsync(timestamp).ConfigureAwait(false);

                    if (properties.RateLimitHandling.HasFlag((RateLimitHandling)scope))
                        continue;
                }
                else
                {
                    await _rateLimitManager.ExchangeRouteRateLimiterAsync(route, null, null).ConfigureAwait(false);
                    await rateLimiter.IndicateExchangeAsync(timestamp).ConfigureAwait(false);

                    if (properties.RateLimitHandling.HasFlag((RateLimitHandling)scope))
                    {
                        if (scope is RateLimitScope.Shared)
                            await Task.Delay(headers.RetryAfter!.Delta.GetValueOrDefault()).ConfigureAwait(false);

                        continue;
                    }
                }

                throw new RateLimitedException(timestamp + (long)headers.RetryAfter!.Delta.GetValueOrDefault().TotalMilliseconds, scope);
            }
            else
            {
                await _rateLimitManager.ExchangeRouteRateLimiterAsync(route, null, null).ConfigureAwait(false);
                await rateLimiter.IndicateExchangeAsync(timestamp).ConfigureAwait(false);
            }

            if (response.IsSuccessStatusCode)
                return response;

            var content = response.Content;
            if (content.Headers.ContentType is { MediaType: "application/json" })
            {
                RestError error;
                try
                {
                    error = (await JsonSerializer.DeserializeAsync(await content.ReadAsStreamAsync().ConfigureAwait(false), Serialization.Default.RestError).ConfigureAwait(false))!;
                }
                catch
                {
                    throw new RestException(response.StatusCode, response.ReasonPhrase!);
                }
                throw new RestException(response.StatusCode, response.ReasonPhrase!, error);
            }
            else
                throw new RestException(response.StatusCode, response.ReasonPhrase!);
        }
    }

    private async ValueTask<IGlobalRateLimiter> AcquireGlobalRateLimiterAsync(RequestProperties properties)
    {
        while (true)
        {
            var rateLimiter = await _rateLimitManager.GetGlobalRateLimiterAsync().ConfigureAwait(false);
            var info = await rateLimiter.TryAcquireAsync().ConfigureAwait(false);
            if (info.RateLimited)
            {
                if (properties.RateLimitHandling.HasFlag(RateLimitHandling.RetryGlobal))
                    await Task.Delay(info.ResetAfter).ConfigureAwait(false);
                else
                    throw new RateLimitedException(Environment.TickCount64 + info.ResetAfter, RateLimitScope.Global);
            }
            else if (info.AlwaysRetry)
                continue;
            else
                return rateLimiter;
        }
    }

    private async ValueTask<IRouteRateLimiter> AcquireRouteRateLimiterAsync(Route route, RequestProperties properties)
    {
        while (true)
        {
            var rateLimiter = await _rateLimitManager.GetRouteRateLimiterAsync(route).ConfigureAwait(false);
            var info = await rateLimiter.TryAcquireAsync().ConfigureAwait(false);
            if (info.RateLimited)
            {
                if (properties.RateLimitHandling.HasFlag(RateLimitHandling.RetryUser))
                    await Task.Delay(info.ResetAfter).ConfigureAwait(false);
                else
                    throw new RateLimitedException(Environment.TickCount64 + info.ResetAfter, RateLimitScope.User);
            }
            else if (info.AlwaysRetry)
                continue;
            else
                return rateLimiter;
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
        _rateLimitManager.Dispose();
    }
}
