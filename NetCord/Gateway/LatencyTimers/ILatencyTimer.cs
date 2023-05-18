namespace NetCord.Gateway.LatencyTimers;

public interface ILatencyTimer
{
    public TimeSpan Elapsed { get; }

    public void Start();
}
