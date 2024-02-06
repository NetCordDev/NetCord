namespace NetCord.Rest;

public interface IRestMessageHandler : IDisposable
{
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);

    public void AddDefaultRequestHeader(string name, string? value);
}
