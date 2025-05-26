namespace NetCord.Logging;

internal static class LoggerHelpers
{
    public static TimeSpan GetRoundedTime(TimeProvider timeProvider)
    {
        var ticks = timeProvider.GetLocalNow().TimeOfDay.Ticks;
        return new(ticks - (ticks % TimeSpan.TicksPerSecond));
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
}
