using NetCord.Rest;

namespace NetCord.Hosting.Rest;

internal sealed class MicrosoftExtensionsRestRequestHandler(HttpClient httpClient) : IRestRequestHandler
{
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default) => httpClient.SendAsync(request, cancellationToken);

    public void AddDefaultHeader(string name, IEnumerable<string> values)
    {
        httpClient.DefaultRequestHeaders.Add(name, values);
    }

    public void Dispose()
    {
        httpClient.Dispose();
    }
}
