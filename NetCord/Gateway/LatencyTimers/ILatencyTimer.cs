namespace NetCord.Gateway;

public interface ILatencyTimer
{
    public TimeSpan Elapsed { get; }

    public void Start();
}
