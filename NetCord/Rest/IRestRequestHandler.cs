namespace NetCord.Rest;

public interface IRestRequestHandler : IDisposable
{
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);

    public void AddDefaultHeader(string name, IEnumerable<string> values);
}
