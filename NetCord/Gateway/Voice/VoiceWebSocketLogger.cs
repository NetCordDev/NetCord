using NetCord.Logging;

namespace NetCord.Gateway.Voice;

internal sealed class VoiceWebSocketLogger(IVoiceLogger logger) : IWebSocketLogger
{
    public void Log(LogLevel logLevel, Exception? exception, string message, params object?[] args)
    {
        logger.Log(logLevel, exception, message, args);
    }

    public void Dispose()
    {
        logger.Dispose();
    }
}
