namespace NetCord.Rest;

public partial class RestClient : IDisposable
{
    private readonly HttpClient _httpClient;

    private readonly Dictionary<RateLimits.Route, RateLimits.IBucket> _buckets = new();

    internal long _globalRateLimitReset;

    private readonly RateLimits.GlobalBucket _globalBucket;
    private readonly RateLimits.NoRateLimitBucket _noRateLimitBucket;

    public RestClient(Token token)
    {
        _httpClient = new();
        _httpClient.DefaultRequestHeaders.Add("Authorization", token.ToHttpHeader());
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("NetCord");
        _globalBucket = new(this);
        _noRateLimitBucket = new(this);
    }

    public async Task<Stream> SendRequestAsync(HttpMethod method, string partialUrl, RateLimits.Route route, RequestProperties? properties)
    {
        string url = $"{Discord.RestUrl}{partialUrl}";
        HttpResponseMessage response;
        var bucket = GetBucket(route);

        response = await bucket.SendAsync(_httpClient, () =>
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
        string url = $"{Discord.RestUrl}{partialUrl}";
        HttpResponseMessage response;
        var bucket = GetBucket(route);

        response = await bucket.SendAsync(_httpClient, () =>
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
        string url = $"{Discord.RestUrl}{partialUrl}";
        HttpResponseMessage response;

        response = await _globalBucket.SendAsync(_httpClient, () =>
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
        string url = $"{Discord.RestUrl}{partialUrl}";
        HttpResponseMessage response;

        response = await _globalBucket.SendAsync(_httpClient, () =>
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
        string url = $"{Discord.RestUrl}{partialUrl}";
        HttpResponseMessage response;

        response = await _noRateLimitBucket.SendAsync(_httpClient, () =>
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
        string url = $"{Discord.RestUrl}{partialUrl}";
        HttpResponseMessage response;

        response = await _noRateLimitBucket.SendAsync(_httpClient, () =>
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
