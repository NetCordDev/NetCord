namespace NetCord.Rest;

public interface IRestRequestHandler : IDisposable
{
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);

    public void AddDefaultHeader(string name, IEnumerable<string> values);
}
