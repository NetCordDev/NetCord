using NetCord.Logging;

namespace NetCord.Gateway;

internal interface IWebSocketLogger : IDisposable
{
    public void Log(LogLevel logLevel, Exception? exception, string message, params object?[] args);
}
