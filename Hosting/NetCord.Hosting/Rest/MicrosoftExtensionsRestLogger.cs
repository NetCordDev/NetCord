using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NetCord.Logging;
using NetCord.Rest;

namespace NetCord.Hosting.Rest;

internal class MicrosoftExtensionsRestLogger(IServiceProvider services) : IRestLogger, IGatewayLogger
{
    private readonly ILogger<RestClient> _logger = services.GetRequiredService<ILogger<RestClient>>();

    void IRestLogger.Log<TState>(NCLogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _logger.Log((MSLogLevel)logLevel, default, state, exception, formatter);
    }

    void IGatewayLogger.Log<TState>(NCLogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
    }
}
