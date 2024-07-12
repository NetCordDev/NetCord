using NetCord.Logging;

namespace NetCord.Gateway.Voice;

public interface IVoiceLogger : IDisposable
{
    public void Log(LogLevel logLevel, Exception? exception, string message, params object?[] args);
}
