using NetCord.Gateway;

using static NetCord.Logging.LoggerHelpers;

namespace NetCord.Logging;

public class TextWriterLogger(TextWriter writer,
                              LogLevel minimumLogLevel = LogLevel.Information,
                              IFormatProvider? formatProvider = null,
                              TimeProvider? timeProvider = null) : IGatewayLogger, IRestLogger, IVoiceLogger
{
    private readonly TimeProvider _timeProvider = timeProvider ?? TimeProvider.System;

    void IGatewayLogger.Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsLogLevelEnabled(logLevel, minimumLogLevel))
            return;

        var value = string.Create(formatProvider,
                                  $"{GetTime(_timeProvider),-12:T}Gateway        {GetConstantSizeLogLevelString(logLevel)}    {formatter(state, exception)}");

        writer.WriteLine(value);
    }

    void IRestLogger.Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsLogLevelEnabled(logLevel, minimumLogLevel))
            return;

        var value = string.Create(formatProvider,
                                  $"{GetTime(_timeProvider),-12:T}Rest           {GetConstantSizeLogLevelString(logLevel)}    {formatter(state, exception)}");

        writer.WriteLine(value);
    }

    void IVoiceLogger.Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsLogLevelEnabled(logLevel, minimumLogLevel))
            return;

        var value = string.Create(formatProvider,
                                  $"{GetTime(_timeProvider),-12:T}Voice          {GetConstantSizeLogLevelString(logLevel)}    {formatter(state, exception)}");

        writer.WriteLine(value);
    }

    bool IGatewayLogger.IsEnabled(LogLevel logLevel)
    {
        return IsLogLevelEnabled(logLevel, minimumLogLevel);
    }

    bool IRestLogger.IsEnabled(LogLevel logLevel)
    {
        return IsLogLevelEnabled(logLevel, minimumLogLevel);
    }

    bool IVoiceLogger.IsEnabled(LogLevel logLevel)
    {
        return IsLogLevelEnabled(logLevel, minimumLogLevel);
    }
}

public class ShardedTextWriterLogger(int shardId,
                                     TextWriter writer,
                                     LogLevel minimumLogLevel = LogLevel.Information,
                                     IFormatProvider? formatProvider = null,
                                     TimeProvider? timeProvider = null) : IGatewayLogger, IRestLogger, IVoiceLogger
{
    private readonly TimeProvider _timeProvider = timeProvider ?? TimeProvider.System;

    public static Func<Shard?, ShardedTextWriterLogger> GetFactory(TextWriter writer,
                                                                   LogLevel minimumLogLevel = LogLevel.Information,
                                                                   IFormatProvider? formatProvider = null,
                                                                   TimeProvider? timeProvider = null)
    {
        return shard => new ShardedTextWriterLogger(shard.GetValueOrDefault().Id, writer, minimumLogLevel, formatProvider, timeProvider);
    }

    void IGatewayLogger.Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsLogLevelEnabled(logLevel, minimumLogLevel))
            return;

        var value = string.Create(formatProvider,
                                  $"{GetTime(_timeProvider),-12:T}Gateway #{shardId,-6}{GetConstantSizeLogLevelString(logLevel)}    {formatter(state, exception)}");

        writer.WriteLine(value);
    }

    void IRestLogger.Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsLogLevelEnabled(logLevel, minimumLogLevel))
            return;

        var value = string.Create(formatProvider,
                                  $"{GetTime(_timeProvider),-12:T}Rest           {GetConstantSizeLogLevelString(logLevel)}    {formatter(state, exception)}");

        writer.WriteLine(value);
    }

    void IVoiceLogger.Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsLogLevelEnabled(logLevel, minimumLogLevel))
            return;

        var value = string.Create(formatProvider,
                                  $"{GetTime(_timeProvider),-12:T}Voice          {GetConstantSizeLogLevelString(logLevel)}    {formatter(state, exception)}");

        writer.WriteLine(value);
    }

    bool IGatewayLogger.IsEnabled(LogLevel logLevel)
    {
        return IsLogLevelEnabled(logLevel, minimumLogLevel);
    }

    bool IRestLogger.IsEnabled(LogLevel logLevel)
    {
        return IsLogLevelEnabled(logLevel, minimumLogLevel);
    }

    bool IVoiceLogger.IsEnabled(LogLevel logLevel)
    {
        return IsLogLevelEnabled(logLevel, minimumLogLevel);
    }
}
