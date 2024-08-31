namespace NetCord.Rest;

public sealed class RestRequestHandler : IRestRequestHandler
{
    private readonly HttpClient _httpClient;

    public RestRequestHandler()
    {
        _httpClient = new();
    }

    public RestRequestHandler(HttpMessageHandler handler)
    {
        _httpClient = new(handler);
    }

    public RestRequestHandler(HttpMessageHandler handler, bool disposeHandler)
    {
        _httpClient = new(handler, disposeHandler);
    }

    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default) => _httpClient.SendAsync(request, cancellationToken);

    public void AddDefaultHeader(string name, IEnumerable<string> values)
    {
        _httpClient.DefaultRequestHeaders.Add(name, values);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
