namespace NetCord.Logging;

public interface IVoiceLogger
{
    public bool IsEnabled(LogLevel logLevel);

    public void Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter);
}
