using NetCord.Gateway;

namespace NetCord.Logging;

public class ConsoleLogger(LogLevel minimumLogLevel = LogLevel.Information,
                           IFormatProvider? formatProvider = null,
                           TimeProvider? timeProvider = null) : TextWriterLogger(Console.Out, minimumLogLevel, formatProvider, timeProvider)
{
}

public class ShardedConsoleLogger(int shardId,
                                  LogLevel minimumLogLevel = LogLevel.Information,
                                  IFormatProvider? formatProvider = null,
                                  TimeProvider? timeProvider = null) : ShardedTextWriterLogger(shardId, Console.Out, minimumLogLevel, formatProvider, timeProvider)
{
    public static Func<Shard?, ShardedConsoleLogger> GetFactory(LogLevel minimumLogLevel = LogLevel.Information,
                                                                IFormatProvider? formatProvider = null,
                                                                TimeProvider? timeProvider = null)
    {
        return shard => new ShardedConsoleLogger(shard.GetValueOrDefault().Id, minimumLogLevel, formatProvider, timeProvider);
    }
}
