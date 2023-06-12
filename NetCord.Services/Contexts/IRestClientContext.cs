using NetCord.Rest;

namespace NetCord.Services;

public interface IRestClientContext
{
    public RestClient Client { get; }
}
