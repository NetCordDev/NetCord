using static NetCord.Logging.LoggerHelpers;

namespace NetCord.Logging;

public class TextWriterLogger(TextWriter writer, LogLevel minimumLogLevel = LogLevel.Information, TimeProvider? timeProvider = null) : IGatewayLogger, IRestLogger, IVoiceLogger
{
    private readonly TimeProvider _timeProvider = timeProvider ?? TimeProvider.System;

    void IGatewayLogger.Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (logLevel < minimumLogLevel)
            return;

        writer.WriteLine($"{GetRoundedTime(_timeProvider)}   Gateway    {GetConstantSizeLogLevelString(logLevel)}   {formatter(state, exception)}");
    }

    void IRestLogger.Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (logLevel < minimumLogLevel)
            return;

        writer.WriteLine($"{GetRoundedTime(_timeProvider)}   Rest       {GetConstantSizeLogLevelString(logLevel)}   {formatter(state, exception)}");
    }

    void IVoiceLogger.Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (logLevel < minimumLogLevel)
            return;

        writer.WriteLine($"{GetRoundedTime(_timeProvider)}   Voice      {GetConstantSizeLogLevelString(logLevel)}   {formatter(state, exception)}");
    }
}

public class ShardedTextWriterLogger(int shardId, TextWriter writer, LogLevel minimumLogLevel = LogLevel.Information, TimeProvider? timeProvider = null) : IGatewayLogger, IRestLogger, IVoiceLogger
{
    private readonly TimeProvider _timeProvider = timeProvider ?? TimeProvider.System;

    void IGatewayLogger.Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (logLevel < minimumLogLevel)
            return;

        writer.WriteLine($"{GetRoundedTime(_timeProvider)}   Gateway #{shardId} {GetConstantSizeLogLevelString(logLevel)}   {formatter(state, exception)}");
    }

    void IRestLogger.Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (logLevel < minimumLogLevel)
            return;

        writer.WriteLine($"{GetRoundedTime(_timeProvider)}   Rest       {GetConstantSizeLogLevelString(logLevel)}   {formatter(state, exception)}");
    }

    void IVoiceLogger.Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (logLevel < minimumLogLevel)
            return;

        writer.WriteLine($"{GetRoundedTime(_timeProvider)}   Voice      {GetConstantSizeLogLevelString(logLevel)}   {formatter(state, exception)}");
    }
}
