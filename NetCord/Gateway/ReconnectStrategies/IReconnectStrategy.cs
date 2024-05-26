namespace NetCord.Gateway.ReconnectStrategies;

public interface IReconnectStrategy
{
    public IEnumerable<TimeSpan> GetDelays();
}
