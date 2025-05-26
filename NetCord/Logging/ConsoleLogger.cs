using NetCord.Gateway;

namespace NetCord.Logging;

public class ConsoleLogger(LogLevel minimumLogLevel = LogLevel.Information, TimeProvider? timeProvider = null) : TextWriterLogger(Console.Out, minimumLogLevel, timeProvider)
{
}

public class ShardedConsoleLogger(int shardId, LogLevel minimumLogLevel = LogLevel.Information, TimeProvider? timeProvider = null) : ShardedTextWriterLogger(shardId, Console.Out, minimumLogLevel, timeProvider)
{
    public static Func<Shard?, ShardedConsoleLogger> GetFactory(LogLevel minimumLogLevel = LogLevel.Information, TimeProvider? timeProvider = null)
    {
        return shard => new ShardedConsoleLogger(shard.GetValueOrDefault().Id, minimumLogLevel, timeProvider);
    }
}
