using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

using NetCord.Gateway;
using NetCord.Logging;
using NetCord.Rest.RateLimits;

namespace NetCord.Rest;

public sealed partial class RestClient : IDisposable
{
    private readonly string _baseUrl;
    private readonly IRestRequestHandler _requestHandler;
    private readonly InternalRestRequestProperties _defaultRequestProperties;
    private readonly IRateLimitManager _rateLimitManager;
    private readonly IRestLogger _logger;

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

    private RestClient(IRestLogger? logger, RestClientConfiguration configuration)
    {
        configuration ??= new();

        _baseUrl = $"https://{configuration.Hostname ?? Discord.RestHostname}/api/v{(int)configuration.Version.GetValueOrDefault(ApiVersion.V10)}";

        var requestHandler = _requestHandler = configuration.RequestHandler ?? new RestRequestHandler();

        requestHandler.AddDefaultHeader("User-Agent", [UserAgentHeader]);

        _defaultRequestProperties = ProcessRequestProperties(configuration.DefaultRequestProperties, requestHandler);

        _rateLimitManager = configuration.RateLimitManager ?? new RateLimitManager();

        _logger = logger ?? NullLogger.Instance;
    }

    public RestClient(RestClientConfiguration? configuration = null) : this((configuration ??= new()).Logger, configuration)
    {
    }

    public RestClient(IToken token, RestClientConfiguration? configuration = null) : this(configuration)
    {
        Token = token;
        _requestHandler.AddDefaultHeader("Authorization", [token.HttpHeaderValue]);
    }

    internal RestClient(IToken token, IRestClientOwnerConfiguration? configuration) : this(GetLogger(configuration, out var restConfiguration), restConfiguration)
    {
        Token = token;
        _requestHandler.AddDefaultHeader("Authorization", [token.HttpHeaderValue]);
    }

    private static IRestLogger? GetLogger(IRestClientOwnerConfiguration? configuration, out RestClientConfiguration outConfiguration)
    {
        if (configuration is null)
        {
            outConfiguration = new();
            return null;
        }

        if (configuration.RestClientConfiguration is { } restClientConfiguration)
        {
            outConfiguration = restClientConfiguration;
            return restClientConfiguration.Logger ?? configuration.Logger;
        }

        outConfiguration = new();
        return configuration.Logger;
    }

    private static InternalRestRequestProperties ProcessRequestProperties(RestRequestProperties? properties, IRestRequestHandler requestHandler)
    {
        if (properties is null)
            return new(RestRateLimitHandling.Retry);

        foreach (var header in properties.GetHeaders())
            requestHandler.AddDefaultHeader(header.Name, header.Values);

        return new(properties.RateLimitHandling.GetValueOrDefault(RestRateLimitHandling.Retry));
    }

    private bool IsEnabled(LogLevel logLevel)
    {
        try
        {
            return _logger.IsEnabled(logLevel);
        }
        catch
        {
            return false;
        }
    }

    private void Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter) where TState : allows ref struct
    {
        try
        {
            _logger.Log(logLevel, state, exception, formatter);
        }
        catch
        {
        }
    }

    public Task<Stream> SendRequestAsync(HttpMethod method, FormattableString route, string? query = null, TopLevelResourceInfo? resourceInfo = null, RestRequestProperties? properties = null, bool global = true, CancellationToken cancellationToken = default)
    {
        var requestProperties = _defaultRequestProperties.Compose(properties);

        var url = $"{_baseUrl}{route}{query}";

        return SendRequestAsync(new(method, route.Format, resourceInfo), route, global, CreateMessage, requestProperties, cancellationToken);

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

        return SendRequestAsync(new(method, route.Format, resourceInfo), route, global, CreateMessage, requestProperties, cancellationToken);

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

    private async Task<Stream> SendRequestAsync(Route route, FormattableString fullRoute, bool global, Func<HttpRequestMessage> messageFunc, InternalRestRequestProperties requestProperties, CancellationToken cancellationToken)
    {
        while (true)
        {
            var globalRateLimiter = global ? await AcquireGlobalRateLimiterAsync(route, requestProperties, cancellationToken).ConfigureAwait(false) : null;

            var rateLimiter = await AcquireRouteRateLimiterAsync(route, requestProperties, cancellationToken).ConfigureAwait(false);

            if (IsEnabled(LogLevel.Trace))
                await LogRequestTraceAsync(route, fullRoute, messageFunc, cancellationToken).ConfigureAwait(false);
            else
                Log(LogLevel.Debug, (Route: route, FullRoute: fullRoute), null, static (s, e) =>
                {
                    return $"Sending a request to route '{s.Route.Method.Method} {s.FullRoute}'.";
                });

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

            if (IsEnabled(LogLevel.Trace))
                await LogResponseTraceAsync(route, fullRoute, response, cancellationToken).ConfigureAwait(false);
            else
                Log(LogLevel.Debug, (Route: route, FullRoute: fullRoute, Response: response), null, static (s, e) =>
                {
                    return $"Received a response from route '{s.Route.Method.Method} {s.FullRoute}'. Status code: {(int)s.Response.StatusCode}.";
                });

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
                        {
                            var retryAfter = headers.RetryAfter!.Delta.GetValueOrDefault();

                            LogRateLimitRetry(route, retryAfter, RateLimitScope.Shared);

                            await Task.Delay(retryAfter, cancellationToken).ConfigureAwait(false);
                        }

                        continue;
                    }

                    throw new RestRateLimitedException(route, (int)headers.RetryAfter!.Delta.GetValueOrDefault().TotalMilliseconds, scope);
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
                        {
                            var retryAfter = headers.RetryAfter!.Delta.GetValueOrDefault();

                            LogRateLimitRetry(route, retryAfter, RateLimitScope.Shared);

                            await Task.Delay(retryAfter, cancellationToken).ConfigureAwait(false);
                        }

                        continue;
                    }
                }

                throw new RestRateLimitedException(route, (int)headers.RetryAfter!.Delta.GetValueOrDefault().TotalMilliseconds, scope);
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

    private async Task LogRequestTraceAsync(Route route, FormattableString fullRoute, Func<HttpRequestMessage> messageFunc, CancellationToken cancellationToken)
    {
        var message = messageFunc();
        string? contentString;
        if (message.Content is { } content)
            contentString = await content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        else
            contentString = null;

        Log(LogLevel.Trace, (Route: route, FullRoute: fullRoute, ContentString: contentString, Message: message), null, static (s, e) =>
        {
            return s.ContentString is { } contentString
                ? $"Sending a request to route '{s.Route.Method.Method} {s.FullRoute}'. Content:{Environment.NewLine}{contentString}"
                : $"Sending a request to route '{s.Route.Method.Method} {s.FullRoute}'.";
        });
    }

    private async Task LogResponseTraceAsync(Route route, FormattableString fullRoute, HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var contentString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        Log(LogLevel.Trace, (Route: route, FullRoute: fullRoute, ContentString: contentString, Response: response), null, static (s, e) =>
        {
            var contentString = s.ContentString;
            return contentString.Length is 0
                ? $"Received a response from route '{s.Route.Method.Method} {s.FullRoute}'. Status code: {(int)s.Response.StatusCode}."
                : $"Received a response from route '{s.Route.Method.Method} {s.FullRoute}'. Status code: {(int)s.Response.StatusCode}. Content:{Environment.NewLine}{contentString}";
        });
    }

    private static void AppendContent(StringBuilder builder, string? contentString)
    {
        builder.AppendLine();
        builder.AppendLine("Content:");
        builder.Append(contentString);
    }

    private async ValueTask<IGlobalRateLimiter> AcquireGlobalRateLimiterAsync(Route route, InternalRestRequestProperties requestProperties, CancellationToken cancellationToken)
    {
        while (true)
        {
            var rateLimiter = await _rateLimitManager.GetGlobalRateLimiterAsync(cancellationToken).ConfigureAwait(false);
            var info = await rateLimiter.TryAcquireAsync(cancellationToken).ConfigureAwait(false);
            if (info.RateLimited)
            {
                if (requestProperties.RateLimitHandling.HasFlag(RestRateLimitHandling.RetryGlobal))
                {
                    LogRateLimitRetry(route, info.ResetAfter, RateLimitScope.Global);

                    await Task.Delay(info.ResetAfter, cancellationToken).ConfigureAwait(false);
                }
                else
                    throw new RestRateLimitedException(route, info.ResetAfter, RateLimitScope.Global);
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
                {
                    LogRateLimitRetry(route, info.ResetAfter, RateLimitScope.User);

                    await Task.Delay(info.ResetAfter, cancellationToken).ConfigureAwait(false);
                }
                else
                    throw new RestRateLimitedException(route, info.ResetAfter, RateLimitScope.User);
            }
            else if (info.AlwaysRetry)
                continue;
            else
                return rateLimiter;
        }
    }

    private void LogRateLimitRetry(Route route, TimeSpan retryAfter, RateLimitScope scope)
    {
        Log(LogLevel.Warning, (Route: route, RetryAfter: retryAfter, Scope: scope), null, static (s, e) =>
        {
            return $"{RateLimitScopeHelpers.GetString(s.Scope)} rate limit exceeded for route '{s.Route}'. Retrying after {s.RetryAfter.TotalMilliseconds:F0} ms.";
        });
    }

    private void LogRateLimitRetry(Route route, int retryAfter, RateLimitScope scope)
    {
        Log(LogLevel.Warning, (Route: route, RetryAfter: retryAfter, Scope: scope), null, static (s, e) =>
        {
            return $"{RateLimitScopeHelpers.GetString(s.Scope)} rate limit exceeded for route '{s.Route}'. Retrying after {s.RetryAfter} ms.";
        });
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
