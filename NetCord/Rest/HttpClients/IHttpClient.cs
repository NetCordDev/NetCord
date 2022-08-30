namespace NetCord.Rest.HttpClients;

public interface IHttpClient : IDisposable
{
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);

    public void AddDefaultRequestHeader(string name, string? value);
}
