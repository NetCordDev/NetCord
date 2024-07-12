using NetCord.Logging;

namespace NetCord.Gateway;

public interface IGatewayLogger : IDisposable
{
    public void Log(LogLevel logLevel, Exception? exception, string message, params object?[] args);
}
