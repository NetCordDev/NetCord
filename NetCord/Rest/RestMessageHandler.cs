namespace NetCord.Rest;

public class RestMessageHandler : IRestMessageHandler
{
    private readonly HttpClient _httpClient;

    public RestMessageHandler()
    {
        _httpClient = new();
    }

    public RestMessageHandler(HttpMessageHandler handler)
    {
        _httpClient = new(handler);
    }

    public RestMessageHandler(HttpMessageHandler handler, bool disposeHandler)
    {
        _httpClient = new(handler, disposeHandler);
    }

    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request) => _httpClient.SendAsync(request);

    public void AddDefaultRequestHeader(string name, string? value)
    {
        _httpClient.DefaultRequestHeaders.Add(name, value);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
