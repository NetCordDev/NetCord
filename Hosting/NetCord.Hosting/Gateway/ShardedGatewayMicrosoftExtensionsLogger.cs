using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Logging;

namespace NetCord.Hosting.Gateway;

internal class ShardedGatewayMicrosoftExtensionsLogger(int shardId, IServiceProvider services) : IGatewayLogger
{
    private readonly ILogger<GatewayClient> _gatewayLogger = services.GetRequiredService<ILogger<GatewayClient>>();
    private readonly EventId _eventId = new(shardId);

    void IGatewayLogger.Log<TState>(NCLogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _gatewayLogger.Log((MSLogLevel)logLevel, _eventId, state, exception, formatter);
    }
}
