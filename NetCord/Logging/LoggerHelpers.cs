namespace NetCord.Logging;

internal static class LoggerHelpers
{
    public static TimeOnly GetTime(TimeProvider timeProvider)
    {
        return TimeOnly.FromTimeSpan(timeProvider.GetLocalNow().TimeOfDay);
    }

    public static string GetConstantSizeLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "Trace",
            LogLevel.Debug => "Debug",
            LogLevel.Information => "Info ",
            LogLevel.Warning => "Warn ",
            LogLevel.Error => "Fail ",
            LogLevel.Critical => "Crit ",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };
    }

    public static bool IsLogLevelEnabled(LogLevel logLevel, LogLevel minimumLogLevel)
    {
        return logLevel >= minimumLogLevel;
    }
}
