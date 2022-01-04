using System.Text.Json;

namespace NetCord;

public partial class RestClient
{
    private readonly HttpClient _httpClient;

    public ChannelModule Channel { get; }
    public GuildModule Guild { get; }
    public InteractionModule Interaction { get; }
    public MessageModule Message { get; }
    public UserModule User { get; }

    public RestClient(string token, TokenType tokenType)
    {
        _httpClient = new();
        _httpClient.DefaultRequestHeaders.Add("Authorization", tokenType == TokenType.Bearer ? token : $"{tokenType} {token}");
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("NetCord");

        Channel = new(this);
        Guild = new(this);
        Interaction = new(this);
        Message = new(this);
        User = new(this);
    }

    public async Task<JsonDocument?> SendRequestAsync(HttpMethod method, string partialUrl, RequestOptions? options)
    {
        string url = Discord.RestUrl + partialUrl;
        HttpResponseMessage? response;
        if (options != null)
        {
            while (true)
            {
                HttpRequestMessage requestMessage = new(method, url);
                options.AddHeaders(requestMessage.Headers);
                response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);
                var retryAfter = response.Headers.RetryAfter;
                if (retryAfter != null)
                    await Task.Delay(retryAfter.Delta!.GetValueOrDefault()).ConfigureAwait(false);
                else
                    break;
            }
        }
        else
        {
            while (true)
            {
                HttpRequestMessage requestMessage = new(method, url);
                response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);
                var retryAfter = response.Headers.RetryAfter;
                if (retryAfter != null)
                    await Task.Delay(retryAfter.Delta!.GetValueOrDefault()).ConfigureAwait(false);
                else
                    break;
            }
        }
        var s = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        if (s.Length == 0)
            return null;
        else
        {
            var json = JsonDocument.Parse(s);
            if (!IsErrorResponse(json))
                return json;
            else
                throw new HttpException(json);
        }
    }

    public async Task<JsonDocument?> SendRequestAsync(HttpMethod method, HttpContent content, string partialUrl, RequestOptions? options)
    {
        string url = Discord.RestUrl + partialUrl;
        HttpResponseMessage? response;
        if (options != null)
        {
            if (options.RetryMode == RetryMode.Always)
            {
                while (true)
                {
                    HttpRequestMessage requestMessage = new(method, url)
                    {
                        Content = content,
                    };
                    options.AddHeaders(requestMessage.Headers);
                    response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);
                    var retryAfter = response.Headers.RetryAfter;
                    if (retryAfter != null)
                        await Task.Delay(retryAfter.Delta!.GetValueOrDefault()).ConfigureAwait(false);
                    else
                        break;
                }
            }
            else if (options.RetryMode == RetryMode.Once)
            {
                HttpRequestMessage requestMessage = new(method, url)
                {
                    Content = content,
                };
                options.AddHeaders(requestMessage.Headers);
                response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);
                var retryAfter = response.Headers.RetryAfter;
                if (retryAfter != null)
                {
                    await Task.Delay(retryAfter.Delta!.GetValueOrDefault()).ConfigureAwait(false);
                    requestMessage = new(method, url)
                    {
                        Content = content,
                    };
                    options.AddHeaders(requestMessage.Headers);
                    response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);
                }
            }
            else
            {
                HttpRequestMessage requestMessage = new(method, url)
                {
                    Content = content,
                };
                options.AddHeaders(requestMessage.Headers);
                response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);
            }
        }
        else
        {
            while (true)
            {
                HttpRequestMessage requestMessage = new(method, url)
                {
                    Content = content,
                };
                response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);
                var retryAfter = response.Headers.RetryAfter;
                if (retryAfter != null)
                    await Task.Delay(retryAfter.Delta!.GetValueOrDefault()).ConfigureAwait(false);
                else
                    break;
            }
        }
        var s = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        if (s.Length == 0)
            return null;
        else
        {
            var json = JsonDocument.Parse(s);
            if (!IsErrorResponse(json))
                return json;
            else
                throw new HttpException(json);
        }
    }

    private static bool IsErrorResponse(JsonDocument response)
    {
        var element = response.RootElement;
        return element.ValueKind == JsonValueKind.Object && element.TryGetProperty("code", out _);
    }
}