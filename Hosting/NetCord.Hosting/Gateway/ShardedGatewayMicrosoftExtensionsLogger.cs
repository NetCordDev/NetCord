using Microsoft.Extensions.Logging;

using NetCord.Logging;

using NetCord.Gateway;
using NetCord.Rest;
using Microsoft.Extensions.DependencyInjection;

namespace NetCord.Hosting.Gateway;

internal class ShardedGatewayMicrosoftExtensionsLogger(Shard shard, IServiceProvider services) : IGatewayLogger, IRestLogger
{
    private readonly ILogger<GatewayClient> _gatewayLogger = services.GetRequiredService<ILogger<GatewayClient>>();
    private readonly ILogger<RestClient> _restLogger = services.GetRequiredService<ILogger<RestClient>>();
    private readonly EventId _eventId = new(shard.Id);

    void IGatewayLogger.Log<TState>(NCLogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _gatewayLogger.Log((MSLogLevel)logLevel, _eventId, state, exception, formatter);
    }

    void IRestLogger.Log<TState>(NCLogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _restLogger.Log((MSLogLevel)logLevel, default, state, exception, formatter);
    }
}
