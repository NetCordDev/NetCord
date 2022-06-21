namespace NetCord.Rest;

public partial class RestClient : IDisposable
{
    private readonly HttpClient _httpClient;

    public RestClient(string token, TokenType tokenType)
    {
        _httpClient = new();
        _httpClient.DefaultRequestHeaders.Add("Authorization", tokenType == TokenType.Bearer ? token : $"{tokenType} {token}");
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("NetCord");
    }

    public async Task<Stream> SendRequestAsync(HttpMethod method, string partialUrl, RequestProperties? options)
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
        if (!response.IsSuccessStatusCode)
            throw new RestException(response);

        return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> SendRequestAsync(HttpMethod method, HttpContent content, string partialUrl, RequestProperties? options)
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
        if (!response.IsSuccessStatusCode)
            throw new RestException(response);

        return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}