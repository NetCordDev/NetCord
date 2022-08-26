namespace NetCord.Gateway;

internal class LatencyTimer
{
    private int _startInterval;

    public void Start()
    {
        _startInterval = Environment.TickCount;
    }

    public int Elapsed => Environment.TickCount - _startInterval;
}
