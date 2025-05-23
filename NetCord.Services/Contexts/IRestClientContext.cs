using NetCord.Rest;

namespace NetCord.Services;

public interface IRestClientContext : IContext
{
    public RestClient Client { get; }
}
