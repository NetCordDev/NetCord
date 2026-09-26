using NetCord.Logging;
using NetCord.Rest;

namespace NetCord.Gateway;

internal interface IRestClientOwnerConfiguration
{
    public RestClientConfiguration? RestClientConfiguration { get; }

    public IRestLogger? Logger { get; }
}
