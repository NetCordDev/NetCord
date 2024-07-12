using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text;

using NetCord.Gateway;
using NetCord.Gateway.Voice;
using NetCord.Rest;

namespace NetCord.Logging;

public class TextWriterLogger(LogLevel minimumLogLevel, TextWriter writer) : IGatewayLogger, IRestLogger, IVoiceLogger
{
    [ThreadStatic]
    private static StringBuilder? _builder;

    void IGatewayLogger.Log(LogLevel logLevel, Exception? exception, string message, params object?[] args)
    {
        Log(logLevel, exception, message, args, "gtw");
    }

    void IRestLogger.Log(LogLevel logLevel, Exception? exception, string message, params object?[] args)
    {
        Log(logLevel, exception, message, args, "rst");
    }

    void IVoiceLogger.Log(LogLevel logLevel, Exception? exception, string message, params object?[] args)
    {
        Log(logLevel, exception, message, args, "vce");
    }

    private void Log(LogLevel logLevel, Exception? exception, string message, object?[] args, string scope)
    {
        if (logLevel < minimumLogLevel)
            return;

        var builder = _builder ??= new();

        builder.Append('[');
        builder.Append(DateTime.Now.ToString("T"));
        builder.Append("] ");
        builder.Append(GetLogLevelString(logLevel));
        builder.Append(' ');
        builder.Append(scope);
        builder.Append(":\t");
        builder.AppendFormat(message, args);

        if (exception is not null)
        {
            builder.Append(writer.NewLine);
            builder.Append(exception);
        }

        writer.WriteLine(builder);

        builder.Clear();
    }

    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            writer.Dispose();
    }
}
