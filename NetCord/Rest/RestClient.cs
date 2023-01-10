using NetCord.Rest.HttpClients;

namespace NetCord.Rest;

public partial class RestClient : IDisposable
{
    private readonly Dictionary<RateLimits.Route, RateLimits.IBucket> _buckets = new();
    private readonly RateLimits.GlobalBucket _globalBucket;
    private readonly RateLimits.NoRateLimitBucket _noRateLimitBucket;
    private readonly string _baseUrl;

    internal readonly IHttpClient _httpClient;

    internal readonly object _globalRateLimitLock = new();
    internal long _globalRateLimitReset;

    public RestClient(RestClientConfiguration? configuration = null)
    {
        configuration ??= new();
        _httpClient = configuration.HttpClient ?? new HttpClients.HttpClient();
        _baseUrl = $"https://{configuration.Hostname ?? Discord.RestHostname}/api/v{(int)configuration.Version}";

        _globalBucket = new(this);
        _noRateLimitBucket = new(this);

        _httpClient.AddDefaultRequestHeader("User-Agent", "NetCord");
    }

    public RestClient(Token token, RestClientConfiguration? configuration = null) : this(configuration)
    {
        _httpClient.AddDefaultRequestHeader("Authorization", token.ToHttpHeader());
    }

    public async Task<Stream> SendRequestAsync(HttpMethod method, string partialUrl, RateLimits.Route route, RequestProperties? properties)
    {
        string url = $"{_baseUrl}{partialUrl}";
        HttpResponseMessage response;
        var bucket = GetBucket(route);

        response = await bucket.SendAsync(() =>
        {
            HttpRequestMessage requestMessage = new(method, url);
            properties?.AddHeaders(requestMessage.Headers);
            return requestMessage;
        }, properties).ConfigureAwait(false);

        //Console.WriteLine($"{method} {partialUrl}: {(response.Headers.TryGetValues("x-ratelimit-bucket", out var values) ? values.First() : "None"),50}");

        return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> SendRequestAsync(HttpMethod method, string partialUrl, RateLimits.Route route, HttpContent content, RequestProperties? properties)
    {
        string url = $"{_baseUrl}{partialUrl}";
        HttpResponseMessage response;
        var bucket = GetBucket(route);

        response = await bucket.SendAsync(() =>
        {
            HttpRequestMessage requestMessage = new(method, url)
            {
                Content = content,
            };
            properties?.AddHeaders(requestMessage.Headers);
            return requestMessage;
        }, properties).ConfigureAwait(false);

        //Console.WriteLine($"{method} {partialUrl}: {(response.Headers.TryGetValues("x-ratelimit-bucket", out var values) ? values.First() : "None"),50}");

        return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> SendRequestAsync(HttpMethod method, string partialUrl, RequestProperties? properties)
    {
        string url = $"{_baseUrl}{partialUrl}";
        HttpResponseMessage response;

        response = await _globalBucket.SendAsync(() =>
        {
            HttpRequestMessage requestMessage = new(method, url);
            properties?.AddHeaders(requestMessage.Headers);
            return requestMessage;
        }, properties).ConfigureAwait(false);

        //Console.WriteLine($"{method} {partialUrl}: {(response.Headers.TryGetValues("x-ratelimit-bucket", out var values) ? values.First() : "None"),50}");

        return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> SendRequestAsync(HttpMethod method, string partialUrl, HttpContent content, RequestProperties? properties)
    {
        string url = $"{_baseUrl}{partialUrl}";
        HttpResponseMessage response;

        response = await _globalBucket.SendAsync(() =>
        {
            HttpRequestMessage requestMessage = new(method, url)
            {
                Content = content,
            };
            properties?.AddHeaders(requestMessage.Headers);
            return requestMessage;
        }, properties).ConfigureAwait(false);

        //Console.WriteLine($"{method} {partialUrl}: {(response.Headers.TryGetValues("x-ratelimit-bucket", out var values) ? values.First() : "None"),50}");

        return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> SendRequestWithoutRateLimitAsync(HttpMethod method, string partialUrl, RequestProperties? properties)
    {
        string url = $"{_baseUrl}{partialUrl}";
        HttpResponseMessage response;

        response = await _noRateLimitBucket.SendAsync(() =>
        {
            HttpRequestMessage requestMessage = new(method, url);
            properties?.AddHeaders(requestMessage.Headers);
            return requestMessage;
        }, properties).ConfigureAwait(false);

        //Console.WriteLine($"{method} {partialUrl}: {(response.Headers.TryGetValues("x-ratelimit-bucket", out var values) ? values.First() : "None"),50}");

        return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> SendRequestWithoutRateLimitAsync(HttpMethod method, string partialUrl, HttpContent content, RequestProperties? properties)
    {
        string url = $"{_baseUrl}{partialUrl}";
        HttpResponseMessage response;

        response = await _noRateLimitBucket.SendAsync(() =>
        {
            HttpRequestMessage requestMessage = new(method, url)
            {
                Content = content,
            };
            properties?.AddHeaders(requestMessage.Headers);
            return requestMessage;
        }, properties).ConfigureAwait(false);

        //Console.WriteLine($"{method} {partialUrl}: {(response.Headers.TryGetValues("x-ratelimit-bucket", out var values) ? values.First() : "None"),50}");

        return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    private RateLimits.IBucket GetBucket(RateLimits.Route route)
    {
        RateLimits.IBucket? bucket;
        lock (_buckets)
        {
            if (!_buckets.TryGetValue(route, out bucket))
            {
                if (route.GlobalRateLimit)
                    bucket = new RateLimits.RouteBucket(this);
                else
                    bucket = new RateLimits.NonGlobalRouteBucket(this);
                _buckets.Add(route, bucket);
            }
        }
        return bucket;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
