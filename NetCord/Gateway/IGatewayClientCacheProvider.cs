using NetCord.Rest;

namespace NetCord.Gateway;

public interface IGatewayClientCacheProvider
{
    public IGatewayClientCache Create(ulong clientId, RestClient client);
}
