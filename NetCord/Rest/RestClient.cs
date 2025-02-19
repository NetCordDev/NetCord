using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;

using NetCord.Rest.RateLimits;

namespace NetCord.Rest;

public sealed partial class RestClient : IDisposable
{
    private readonly string _baseUrl;
    private readonly IRestRequestHandler _requestHandler;
    private readonly InternalRestRequestProperties _defaultRequestProperties;
    private readonly IRateLimitManager _rateLimitManager;

    private readonly record struct InternalRestRequestProperties(RestRateLimitHandling RateLimitHandling)
    {
        public InternalRestRequestProperties Compose(RestRequestProperties? properties)
        {
            return properties is null ? this : new(properties.RateLimitHandling.GetValueOrDefault(RateLimitHandling));
        }
    }

    /// <summary>
    /// The token of the <see cref="RestClient"/>.
    /// </summary>
    public IToken? Token { get; }

    public RestClient(RestClientConfiguration? configuration = null)
    {
        configuration ??= new();

        _baseUrl = $"https://{configuration.Hostname ?? Discord.RestHostname}/api/v{(int)configuration.Version.GetValueOrDefault(ApiVersion.V10)}";

        var requestHandler = _requestHandler = configuration.RequestHandler ?? new RestRequestHandler();

        requestHandler.AddDefaultHeader("User-Agent", [UserAgentHeader]);

        _defaultRequestProperties = ProcessRequestProperties(configuration.DefaultRequestProperties, requestHandler);

        _rateLimitManager = configuration.RateLimitManager ?? new RateLimitManager();
    }

    private static InternalRestRequestProperties ProcessRequestProperties(RestRequestProperties? properties, IRestRequestHandler requestHandler)
    {
        if (properties is null)
            return new(RestRateLimitHandling.Retry);

        foreach (var header in properties.GetHeaders())
            requestHandler.AddDefaultHeader(header.Name, header.Values);

        return new(properties.RateLimitHandling.GetValueOrDefault(RestRateLimitHandling.Retry));
    }

    public RestClient(IToken token, RestClientConfiguration? configuration = null) : this(configuration)
    {
        Token = token;
        _requestHandler.AddDefaultHeader("Authorization", [token.HttpHeaderValue]);
    }

    public Task<Stream> SendRequestAsync(HttpMethod method, FormattableString route, string? query = null, TopLevelResourceInfo? resourceInfo = null, RestRequestProperties? properties = null, bool global = true, CancellationToken cancellationToken = default)
    {
        var requestProperties = _defaultRequestProperties.Compose(properties);

        var url = $"{_baseUrl}{route}{query}";

        return SendRequestAsync(new(method, route.Format, resourceInfo), global, CreateMessage, requestProperties, cancellationToken);

        HttpRequestMessage CreateMessage()
        {
            HttpRequestMessage requestMessage = new(method, url);

            if (properties is not null)
            {
                var headers = requestMessage.Headers;
                foreach (var header in properties.GetHeaders())
                    headers.Add(header.Name, header.Values);
            }

            return requestMessage;
        }
    }

    public Task<Stream> SendRequestAsync(HttpMethod method, HttpContent content, FormattableString route, string? query = null, TopLevelResourceInfo? resourceInfo = null, RestRequestProperties? properties = null, bool global = true, CancellationToken cancellationToken = default)
    {
        var requestProperties = _defaultRequestProperties.Compose(properties);

        var url = $"{_baseUrl}{route}{query}";

        return SendRequestAsync(new(method, route.Format, resourceInfo), global, CreateMessage, requestProperties, cancellationToken);

        HttpRequestMessage CreateMessage()
        {
            HttpRequestMessage requestMessage = new(method, url)
            {
                Content = content,
            };

            if (properties is not null)
            {
                var headers = requestMessage.Headers;
                foreach (var header in properties.GetHeaders())
                    headers.Add(header.Name, header.Values);
            }

            return requestMessage;
        }
    }

    private async Task<Stream> SendRequestAsync(Route route, bool global, Func<HttpRequestMessage> messageFunc, InternalRestRequestProperties requestProperties, CancellationToken cancellationToken)
    {
        while (true)
        {
            var globalRateLimiter = global ? await AcquireGlobalRateLimiterAsync(requestProperties, cancellationToken).ConfigureAwait(false) : null;

            var rateLimiter = await AcquireRouteRateLimiterAsync(route, requestProperties, cancellationToken).ConfigureAwait(false);

            var timestamp = Environment.TickCount64;
            HttpResponseMessage response;
            try
            {
                response = await _requestHandler.SendAsync(messageFunc(), cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                await rateLimiter.CancelAcquireAsync(timestamp, default).ConfigureAwait(false);
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
                        await rateLimiter.UpdateAsync(rateLimitInfo, default).ConfigureAwait(false);
                    else
                    {
                        await _rateLimitManager.ExchangeRouteRateLimiterAsync(route, rateLimitInfo, rateLimiterBucketInfo, default).ConfigureAwait(false);
                        await rateLimiter.IndicateExchangeAsync(timestamp, default).ConfigureAwait(false);
                    }
                }
                else
                {
                    await _rateLimitManager.ExchangeRouteRateLimiterAsync(route, rateLimitInfo, null, default).ConfigureAwait(false);
                    await rateLimiter.IndicateExchangeAsync(timestamp, default).ConfigureAwait(false);
                }

                if (rateLimited)
                {
                    if (requestProperties.RateLimitHandling.HasFlag((RestRateLimitHandling)scope))
                    {
                        if (scope is RateLimitScope.Shared)
                            await Task.Delay(headers.RetryAfter!.Delta.GetValueOrDefault(), cancellationToken).ConfigureAwait(false);

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
                        await globalRateLimiter!.IndicateRateLimitAsync(timestamp + (long)headers.RetryAfter!.Delta.GetValueOrDefault().TotalMilliseconds, default).ConfigureAwait(false);

                    await rateLimiter.CancelAcquireAsync(timestamp, default).ConfigureAwait(false);

                    if (requestProperties.RateLimitHandling.HasFlag((RestRateLimitHandling)scope))
                        continue;
                }
                else
                {
                    await _rateLimitManager.ExchangeRouteRateLimiterAsync(route, null, null, default).ConfigureAwait(false);
                    await rateLimiter.IndicateExchangeAsync(timestamp, default).ConfigureAwait(false);

                    if (requestProperties.RateLimitHandling.HasFlag((RestRateLimitHandling)scope))
                    {
                        if (scope is RateLimitScope.Shared)
                            await Task.Delay(headers.RetryAfter!.Delta.GetValueOrDefault(), cancellationToken).ConfigureAwait(false);

                        continue;
                    }
                }

                throw new RateLimitedException(timestamp + (long)headers.RetryAfter!.Delta.GetValueOrDefault().TotalMilliseconds, scope);
            }
            else
            {
                await _rateLimitManager.ExchangeRouteRateLimiterAsync(route, null, null, default).ConfigureAwait(false);
                await rateLimiter.IndicateExchangeAsync(timestamp, default).ConfigureAwait(false);
            }

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

            var content = response.Content;

            if (content.Headers.ContentType is { MediaType: "application/json" })
            {
                RestError error;
                try
                {
                    error = (await JsonSerializer.DeserializeAsync(await content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false), Serialization.Default.RestError, cancellationToken).ConfigureAwait(false))!;
                }
                catch (JsonException)
                {
                    throw new RestException(response.StatusCode, response.ReasonPhrase);
                }
                throw new RestException(response.StatusCode, response.ReasonPhrase, error);
            }
            else
                throw new RestException(response.StatusCode, response.ReasonPhrase);
        }
    }

    private async ValueTask<IGlobalRateLimiter> AcquireGlobalRateLimiterAsync(InternalRestRequestProperties requestProperties, CancellationToken cancellationToken)
    {
        while (true)
        {
            var rateLimiter = await _rateLimitManager.GetGlobalRateLimiterAsync(cancellationToken).ConfigureAwait(false);
            var info = await rateLimiter.TryAcquireAsync(cancellationToken).ConfigureAwait(false);
            if (info.RateLimited)
            {
                if (requestProperties.RateLimitHandling.HasFlag(RestRateLimitHandling.RetryGlobal))
                    await Task.Delay(info.ResetAfter, cancellationToken).ConfigureAwait(false);
                else
                    throw new RateLimitedException(Environment.TickCount64 + info.ResetAfter, RateLimitScope.Global);
            }
            else if (info.AlwaysRetry)
                continue;
            else
                return rateLimiter;
        }
    }

    private async ValueTask<IRouteRateLimiter> AcquireRouteRateLimiterAsync(Route route, InternalRestRequestProperties requestProperties, CancellationToken cancellationToken)
    {
        while (true)
        {
            var rateLimiter = await _rateLimitManager.GetRouteRateLimiterAsync(route, cancellationToken).ConfigureAwait(false);
            var info = await rateLimiter.TryAcquireAsync(cancellationToken).ConfigureAwait(false);
            if (info.RateLimited)
            {
                if (requestProperties.RateLimitHandling.HasFlag(RestRateLimitHandling.RetryUser))
                    await Task.Delay(info.ResetAfter, cancellationToken).ConfigureAwait(false);
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
        _requestHandler.Dispose();
        _rateLimitManager.Dispose();
    }
}
