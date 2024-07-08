namespace NetCord.Gateway.ReconnectStrategies;

public class ReconnectStrategy : IReconnectStrategy
{
    public IEnumerable<TimeSpan> GetDelays()
    {
        yield return default;

        var increment = TimeSpan.FromSeconds(10);

        var delay = increment;

        yield return delay;

        for (int i = 0; i < 5; i++)
            yield return delay += increment;

        while (true)
            yield return delay;
    }
}
