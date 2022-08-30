namespace NetCord.Rest.HttpClients;

public class HttpClient : IHttpClient
{
    private readonly System.Net.Http.HttpClient _httpClient;

    public HttpClient()
    {
        _httpClient = new();
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
