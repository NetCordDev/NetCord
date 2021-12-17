
using System.Text.Json;

namespace NetCord;

public static class CDN
{
    public const string BaseUrl = "https://discord.com/api/v9";

    public static Task<JsonDocument> SendAsync(HttpMethod method, BuiltMessage message, string partialUrl, BotClient client)
    {
        return SendAsync(method, message._content, partialUrl, client)!;
    }

    /// <summary>
    /// Send a <see cref="HttpRequestMessage"/>
    /// </summary>
    /// <param name="method"></param>
    /// <param name="message"></param>
    /// <param name="partialUrl"></param>
    /// <param name="client"></param>
    /// <returns></returns>
    public static async Task<JsonDocument?> SendAsync(HttpMethod method, MultipartFormDataContent content, string partialUrl, BotClient client)
    {
        string url = BaseUrl + partialUrl;
        HttpRequestMessage requestMessage = new(method, url)
        {
            Content = content
        };
        HttpResponseMessage response = await client._httpClient.SendAsync(requestMessage).ConfigureAwait(false);
        var retryAfter = response.Headers.RetryAfter;
        while (retryAfter != null)
        {
            await Task.Delay(retryAfter.Delta!.Value).ConfigureAwait(false);
            requestMessage = new(method, url)
            {
                Content = content
            };
            response = await client._httpClient.SendAsync(requestMessage).ConfigureAwait(false);
            retryAfter = response.Headers.RetryAfter;
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
                throw new Exceptions.HttpException(json);
        }
    }

    public static async Task<JsonDocument?> SendAsync(HttpMethod method, string message, string partialUrl, BotClient client, string reason)
    {
        string url = BaseUrl + partialUrl;
        StringContent stringContent = new(message);
        stringContent.Headers.ContentType!.MediaType = "application/json";
        HttpRequestMessage requestMessage = new(method, url)
        {
            Content = stringContent
        };
        requestMessage.Headers.Add("X-Audit-Log-Reason", reason);
        HttpResponseMessage response = await client._httpClient.SendAsync(requestMessage).ConfigureAwait(false);
        var retryAfter = response.Headers.RetryAfter;
        while (retryAfter != null)
        {
            await Task.Delay(retryAfter.Delta!.Value).ConfigureAwait(false);
            requestMessage = new(method, url)
            {
                Content = stringContent
            };
            response = await client._httpClient.SendAsync(requestMessage).ConfigureAwait(false);
            retryAfter = response.Headers.RetryAfter;
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
                throw new Exceptions.HttpException(json);
        }
    }

    /// <summary>
    /// Send a <see cref="HttpRequestMessage"/>
    /// </summary>
    /// <param name="method"></param>
    /// <param name="message"></param>
    /// <param name="partialUrl"></param>
    /// <param name="client"></param>
    /// <returns></returns>
    public static async Task<JsonDocument?> SendAsync(HttpMethod method, string message, string partialUrl, BotClient client)
    {
        string url = BaseUrl + partialUrl;
        StringContent stringContent = new(message);
        stringContent.Headers.ContentType!.MediaType = "application/json";
        HttpRequestMessage requestMessage = new(method, url)
        {
            Content = stringContent
        };
        HttpResponseMessage response = await client._httpClient.SendAsync(requestMessage).ConfigureAwait(false);
        var retryAfter = response.Headers.RetryAfter;
        while (retryAfter != null)
        {
            await Task.Delay(retryAfter.Delta!.Value).ConfigureAwait(false);
            requestMessage = new(method, url)
            {
                Content = stringContent
            };
            response = await client._httpClient.SendAsync(requestMessage).ConfigureAwait(false);
            retryAfter = response.Headers.RetryAfter;
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
                throw new Exceptions.HttpException(json);
        }
    }

    public static async Task<JsonDocument?> SendAsync(HttpMethod method, string partialUrl, BotClient client, string reason)
    {
        string url = BaseUrl + partialUrl;
        HttpRequestMessage requestMessage = new(method, url);
        requestMessage.Headers.Add("X-Audit-Log-Reason", reason);
        HttpResponseMessage response = await client._httpClient.SendAsync(requestMessage).ConfigureAwait(false);
        var retryAfter = response.Headers.RetryAfter;
        while (retryAfter != null)
        {
            await Task.Delay(retryAfter.Delta!.Value).ConfigureAwait(false);
            requestMessage = new(method, url);
            response = await client._httpClient.SendAsync(requestMessage).ConfigureAwait(false);
            retryAfter = response.Headers.RetryAfter;
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
                throw new Exceptions.HttpException(json);
        }
    }

    public static async Task<JsonDocument?> SendAsync(HttpMethod method, string partialUrl, BotClient client)
    {
        string url = BaseUrl + partialUrl;
        HttpRequestMessage requestMessage = new(method, url);
        HttpResponseMessage response = await client._httpClient.SendAsync(requestMessage).ConfigureAwait(false);
        var retryAfter = response.Headers.RetryAfter;
        while (retryAfter != null)
        {
            await Task.Delay(retryAfter.Delta!.Value).ConfigureAwait(false);
            requestMessage = new(method, url);
            response = await client._httpClient.SendAsync(requestMessage).ConfigureAwait(false);
            retryAfter = response.Headers.RetryAfter;
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
                throw new Exceptions.HttpException(json);
        }
    }

    private static bool IsErrorResponse(JsonDocument response)
    {
        var element = response.RootElement;
        return element.ValueKind == JsonValueKind.Object && element.TryGetProperty("code", out _);
    }
}